using Aiursoft.Archon.SDK.Services;
using Aiursoft.DBTools;
using Aiursoft.DocGenerator.Middlewares;
using Aiursoft.DocGenerator.Services;
using Aiursoft.DocGenerator.Tools;
using Aiursoft.Observer.SDK.Services.ToObserverServer;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.Wiki.Data;
using Aiursoft.Wiki.Models;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aiursoft.Wiki.Services
{
    public class Seeder : ITransientDependency
    {
        private static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);
        private readonly WikiDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly HTTPService _http;
        private readonly MarkDownDocGenerator _markDownGenerator;
        private readonly EventService _eventService;
        private readonly AppsContainer _appsContainer;

        public Seeder(
            WikiDbContext dbContext,
            IConfiguration configuration,
            HTTPService http,
            MarkDownDocGenerator markDownGenerator,
            EventService eventService,
            AppsContainer appsContainer)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _http = http;
            _markDownGenerator = markDownGenerator;
            _eventService = eventService;
            _appsContainer = appsContainer;
        }

        private Task AllClear()
        {
            _dbContext.Article.Delete(t => true);
            _dbContext.Collections.Delete(t => true);
            return _dbContext.SaveChangesAsync();
        }

        public async Task Seed()
        {
            try
            {
                await SemaphoreSlim.WaitAsync();
                await AllClear();
                var result = await _http.Get(new AiurUrl(_configuration["ResourcesUrl"] + "structure.json"), false);
                var sourceObject = JsonConvert.DeserializeObject<List<Collection>>(result);

                // Get all collections
                foreach (var collection in sourceObject)
                {
                    // Insert collection
                    var newCollection = new Collection
                    {
                        CollectionTitle = collection.CollectionTitle,
                        DocAPIAddress = collection.DocAPIAddress
                    };
                    await _dbContext.Collections.AddAsync(newCollection);
                    await _dbContext.SaveChangesAsync();

                    // Get markdown from existing documents.
                    foreach (var article in collection.Articles ?? new List<Article>())
                    {
                        // markdown http url.
                        if (!string.IsNullOrEmpty(article.ArticleAddress))
                        {
                            var newArticle = new Article
                            {
                                ArticleTitle = article.ArticleTitle,
                                ArticleAddress = article.ArticleAddress,
                                ArticleContent = await _http.Get(new AiurUrl(article.ArticleAddress), false),
                                CollectionId = newCollection.CollectionId
                            };
                            await _dbContext.Article.AddAsync(newArticle);
                            await _dbContext.SaveChangesAsync();
                        }
                        // GitHub repo.
                        else
                        {
                            var newArticle = new Article
                            {
                                ArticleTitle = article.ArticleTitle,
                                ArticleContent = await _http.Get(new AiurUrl($"{_configuration["ResourcesUrl"]}{collection.CollectionTitle}/{article.ArticleTitle}.md"), false),
                                CollectionId = newCollection.CollectionId
                            };
                            await _dbContext.Article.AddAsync(newArticle);
                            await _dbContext.SaveChangesAsync();
                        }
                    }

                    // Parse the appended doc.
                    if (!string.IsNullOrWhiteSpace(collection.DocAPIAddress))
                    {
                        var doamin = _configuration["RootDomain"];
                        var docBuilt = collection.DocAPIAddress
                            .Replace("{{rootDomain}}", doamin);
                        // Generate markdown from doc generator
                        var docString = await _http.Get(new AiurUrl(docBuilt), false);
                        var docModel = JsonConvert.DeserializeObject<List<API>>(docString);
                        var docGrouped = docModel.GroupBy(t => t.ControllerName);
                        var apiRoot = docBuilt.ToLower().Replace("/doc", "");
                        foreach (var docController in docGrouped)
                        {
                            var markdown = _markDownGenerator.GenerateMarkDownForAPI(docController, apiRoot);
                            var newArticle = new Article
                            {
                                ArticleTitle = docController.Key.TrimController(),
                                ArticleContent = markdown,
                                CollectionId = newCollection.CollectionId,
                                BuiltByJson = true
                            };
                            await _dbContext.Article.AddAsync(newArticle);
                            await _dbContext.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                var accessToken = await _appsContainer.AccessToken();
                await _eventService.LogExceptionAsync(accessToken, e, "Seeder");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }
    }
}
