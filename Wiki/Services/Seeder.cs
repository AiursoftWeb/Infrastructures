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
        public string _authorized = "<span class=\"badge badge-pill badge-danger\">Authorize</span>";
        public string _unauthorized = "<span class=\"badge badge-pill badge-secondary\">Anonymous</span>";
        public string _post = "<span class=\"badge badge-pill badge-warning text-white\">POST</span>";
        public string _get = "<span class=\"badge badge-pill badge-success\">GET</span>";
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

        private string ArgTypeConverter(ArgumentType type)
        {
            switch (type)
            {
                case ArgumentType.boolean:
                    return "Boolean";
                case ArgumentType.text:
                    return "Text";
                case ArgumentType.number:
                    return "Number";
                case ArgumentType.datetime:
                    return "DateTime";
                case ArgumentType.unknown:
                    return "A magic type!";
            }
            throw new InvalidOperationException(type.ToString());
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
                                content += $"<h3 id='{docAction.ActionName}'>{(docAction.IsPost ? _post : _get)} {(docAction.AuthRequired ? _authorized : string.Empty)} {docAction.ActionName}</h3>\r\n\r\n";
                                content += $"Request path:\r\n\r\n";
                                content += $"\t{collection.DocAPIAddress.ToLower().Replace("/doc", "")}/{docAction.ControllerName.Replace("Controller", "")}/{docAction.ActionName}\r\n\r\n";
                                if (docAction.IsPost)
                                {
                                    content += $"Request content type:\r\n\r\n";
                                    content += $"\tapplication/x-www-form-urlencoded\r\n\r\n";
                                }
                                if (docAction.Arguments.Count > 0)
                                {
                                    content += $"Request {(docAction.IsPost ? "form" : "arguments")}:\r\n\r\n";
                                    foreach (var arg in docAction.Arguments)
                                    {
                                        content += $"\t{arg.Name}{(arg.Required ? "- <b class='text-danger'>Required</b>" : string.Empty)} - Type: <b class='text-danger'>{(ArgTypeConverter(arg.Type))}</b>\r\n";
                                    }
                                    content += $"\r\n";
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
