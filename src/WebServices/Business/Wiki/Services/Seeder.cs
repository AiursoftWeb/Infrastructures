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
using Microsoft.Extensions.Logging;
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
        private static readonly SemaphoreSlim SemaphoreSlim = new(1, 1);
        private readonly WikiDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly HttpService _http;
        private readonly MarkDownDocGenerator _markDownGenerator;
        private readonly EventService _eventService;
        private readonly AppsContainer _appsContainer;
        private readonly ILogger<Seeder> _logger;
        private readonly string _domain;

        public Seeder(
            WikiDbContext dbContext,
            IConfiguration configuration,
            HttpService http,
            MarkDownDocGenerator markDownGenerator,
            EventService eventService,
            AppsContainer appsContainer,
            ILogger<Seeder> logger)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _http = http;
            _markDownGenerator = markDownGenerator;
            _eventService = eventService;
            _appsContainer = appsContainer;
            _logger = logger;
            _domain = configuration["RootDomain"];
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
                var result = await _http.Get(new AiurUrl(_configuration["ResourcesUrl"] + "structure.json"));
                var sourceObject = JsonConvert.DeserializeObject<List<Collection>>(result);

                // Get all collections
                foreach (var collection in sourceObject ?? new List<Collection>())
                {
                    // Insert collection
                    var newCollection = new Collection
                    {
                        CollectionTitle = collection.CollectionTitle,
                        DocAPIAddress = collection.DocAPIAddress?.Replace("{{rootDomain}}", _domain)
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
                                ArticleContent = await _http.Get(new AiurUrl(article.ArticleAddress)),
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
                                ArticleContent = await _http.Get(new AiurUrl($"{_configuration["ResourcesUrl"]}{collection.CollectionTitle}/{article.ArticleTitle}.md")),
                                CollectionId = newCollection.CollectionId
                            };
                            await _dbContext.Article.AddAsync(newArticle);
                            await _dbContext.SaveChangesAsync();
                        }
                    }

                    // Parse the appended doc.
                    if (string.IsNullOrWhiteSpace(collection.DocAPIAddress)) continue;
                    // Generate markdown from doc generator

                    _logger.LogInformation($"Requesting doc API: {collection.DocAPIAddress}");
                    var docString = await _http.Get(new AiurUrl(collection.DocAPIAddress));
                    var docModel = JsonConvert.DeserializeObject<List<API>>(docString);
                    if (docModel == null)
                    {
                        continue;
                    }
                    var docGrouped = docModel.GroupBy(t => t.ControllerName);
                    var apiRoot = collection.DocAPIAddress.ToLower().Replace("/doc", "");
                    foreach (var docController in docGrouped)
                    {
                        var markdown = _markDownGenerator.GenerateMarkDownForApi(docController, apiRoot);
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
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        public async Task HandleException(Exception e)
        {
            var accessToken = await _appsContainer.AccessToken();
            await _eventService.LogExceptionAsync(accessToken, e, "Seeder");
            this._logger.LogCritical(e, e.Message);
        }
    }
}
