using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aiursoft.Directory.SDK.Services;
using Aiursoft.DBTools;
using Aiursoft.DocGenerator.Middlewares;
using Aiursoft.DocGenerator.Services;
using Aiursoft.DocGenerator.Tools;
using Aiursoft.Observer.SDK.Services.ToObserverServer;
using Aiursoft.Scanner.Abstractions;
using Aiursoft.Wiki.Data;
using Aiursoft.Wiki.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Aiursoft.Canon;
using Aiursoft.SDK.Services;

namespace Aiursoft.Wiki.Services;

public class Seeder : ITransientDependency
{
    private static readonly SemaphoreSlim SemaphoreSlim = new(1, 1);
    private readonly DirectoryAppTokenService _directoryAppTokenService;
    private readonly IConfiguration _configuration;
    private readonly RetryEngine _retry;
    private readonly WikiDbContext _dbContext;
    private readonly string _domain;
    private readonly ObserverService _eventService;
    private readonly HttpService _http;
    private readonly ILogger<Seeder> _logger;
    private readonly MarkDownDocGenerator _markDownGenerator;

    public Seeder(
        RetryEngine retry,
        WikiDbContext dbContext,
        IConfiguration configuration,
        HttpService http,
        MarkDownDocGenerator markDownGenerator,
        ObserverService eventService,
        DirectoryAppTokenService directoryAppTokenService,
        ILogger<Seeder> logger)
    {
        _retry = retry;
        _dbContext = dbContext;
        _configuration = configuration;
        _http = http;
        _markDownGenerator = markDownGenerator;
        _eventService = eventService;
        _directoryAppTokenService = directoryAppTokenService;
        _logger = logger;
        _domain = configuration["RootDomain"];
    }

    private Task AllClear()
    {
        _dbContext.Article.Delete(t => true);
        _dbContext.Collections.Delete(t => true);
        return _dbContext.SaveChangesAsync();
    }

    public async Task SeedWithRetry()
    {
        try
        {
            await _retry.RunWithRetry(async _ =>
            {
                await Seed();
            }, attempts: 5);
        }
        catch (Exception e)
        {
            var accessToken = await _directoryAppTokenService.GetAccessTokenAsync();
            await _eventService.LogExceptionAsync(accessToken, e, "Seeder");
            _logger.LogCritical(e, "Failed to seed Wiki database");
            throw;
        }
    }

    public async Task Seed()
    {
        try
        {
            await SemaphoreSlim.WaitAsync();
            await AllClear();
            var result = await _http.Get(_configuration["ResourcesUrl"]);
            var sourceObject = JsonConvert.DeserializeObject<List<Collection>>(result);

            // Get all collections
            foreach (var collection in sourceObject ?? new List<Collection>())
            {
                collection.DocAPIAddress = collection.DocAPIAddress?.Replace("{{rootDomain}}", _domain);
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
                    // markdown http url.
                {
                    if (!string.IsNullOrEmpty(article.ArticleAddress))
                    {
                        var newArticle = new Article
                        {
                            ArticleTitle = article.ArticleTitle,
                            ArticleAddress = article.ArticleAddress,
                            ArticleContent = await _http.Get(article.ArticleAddress),
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
                            ArticleContent = await _http.Get($"{_configuration["ResourcesPath"]}{collection.CollectionTitle}/{article.ArticleTitle}.md"),
                            CollectionId = newCollection.CollectionId
                        };
                        await _dbContext.Article.AddAsync(newArticle);
                        await _dbContext.SaveChangesAsync();
                    }
                }

                // Parse the appended doc.
                if (string.IsNullOrWhiteSpace(collection.DocAPIAddress))
                {
                    continue;
                }
                // Generate markdown from doc generator

                _logger.LogInformation($"Requesting doc API...");
                var docString = await _http.Get(collection.DocAPIAddress);
                var docModel = JsonConvert.DeserializeObject<List<Api>>(docString);
                if (docModel == null)
                {
                    continue;
                }

                var docGrouped = docModel.GroupBy(t => t.ControllerName);
                var apiRoot = collection.DocAPIAddress.ToLower().Replace("/doc", "");
                foreach (var docController in docGrouped)
                {
                    var markdown = _markDownGenerator.GenerateMarkDownForController(docController, apiRoot);
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
}