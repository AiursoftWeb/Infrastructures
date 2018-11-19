using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services;
using Aiursoft.Wiki.Data;
using Aiursoft.Wiki.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Pylon.Middlewares;

namespace Aiursoft.Wiki.Services
{
    public class Seeder
    {
        public bool Seeding { get; set; } = false;
        public string _post = "<span class=\"badge badge-pill badge-warning text-white\">HTTP POST</span>";
        public string _get = "<span class=\"badge badge-pill badge-success\">HTTP GET</span>";
        private readonly WikiDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly HTTPService _http;

        public Seeder(
            WikiDbContext dbContext,
            IConfiguration configuration,
            HTTPService http)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _http = http;
        }

        public async Task Seed()
        {
            try
            {
                Seeding = true;
                _dbContext.Article.Delete(t => true);
                _dbContext.Collections.Delete(t => true);
                await _dbContext.SaveChangesAsync();
                var result = await _http.Get(new AiurUrl(_configuration["ResourcesUrl"] + "structure.json"), false);
                var sourceObject = JsonConvert.DeserializeObject<List<Collection>>(result);
                foreach (var collection in sourceObject)
                {
                    var newCollection = new Collection
                    {
                        CollectionTitle = collection.CollectionTitle
                    };
                    _dbContext.Collections.Add(newCollection);
                    await _dbContext.SaveChangesAsync();
                    foreach (var article in collection.Articles ?? new List<Article>())
                    {
                        var newarticle = new Article
                        {
                            ArticleTitle = article.ArticleTitle,
                            ArticleContent = await _http.Get(new AiurUrl($"{_configuration["ResourcesUrl"]}{collection.CollectionTitle}/{article.ArticleTitle}.md"), false),
                            CollectionId = newCollection.CollectionId
                        };
                        _dbContext.Article.Add(newarticle);
                        await _dbContext.SaveChangesAsync();
                    }
                    if (!string.IsNullOrWhiteSpace(collection.DocAPIAddress))
                    {
                        var docString = await _http.Get(new AiurUrl(collection.DocAPIAddress), false);
                        var docModel = JsonConvert.DeserializeObject<List<API>>(docString);
                        var docGrouped = docModel.GroupBy(t => t.ControllerName);
                        foreach (var docController in docGrouped)
                        {
                            var content = $"# {docController.Key.Replace("Controller", "")}\r\n\r\n";
                            content += $"## Catalog\r\n\r\n";
                            foreach (var docAction in docController)
                            {
                                content += $"* [{docAction.ActionName}](#{docAction.ActionName})\r\n";
                            }
                            content += $"\r\n";
                            foreach (var docAction in docController)
                            {
                                content += $"<h3 id='{docAction.ActionName}'>{docAction.ActionName}</h3>\r\n\r\n";
                                content += $"Request path:\r\n\r\n";
                                content += $"\t/{docAction.ControllerName.Replace("Controller", "")}/{docAction.ActionName}\r\n\r\n";
                                content += $"Request method:\r\n\r\n";
                                content += $"{(docAction.IsPost ? _post : _get)}\r\n\r\n";
                                if (docAction.AuthRequired)
                                {
                                    content += $"Preconditions:\r\n\r\n";
                                    content += $"<span class=\"badge badge-pill badge-danger\">Authorized</span>\r\n\r\n";
                                }
                            }
                            var newarticle = new Article
                            {
                                ArticleTitle = docController.Key.Replace("Controller", ""),
                                ArticleContent = content,
                                CollectionId = newCollection.CollectionId
                            };
                            _dbContext.Article.Add(newarticle);
                            await _dbContext.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                Seeding = false;
            }
        }
    }
}
