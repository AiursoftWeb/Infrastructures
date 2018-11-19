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

        private readonly WikiDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly HTTPService _http;
        private readonly MarkDownGenerator _markdown;

        public Seeder(
            WikiDbContext dbContext,
            IConfiguration configuration,
            HTTPService http)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _http = http;
            _markdown = new MarkDownGenerator();
        }

        public Task AllClear()
        {
            _dbContext.Article.Delete(t => true);
            _dbContext.Collections.Delete(t => true);
            return _dbContext.SaveChangesAsync();
        }

        public async Task Seed()
        {
            try
            {
                Seeding = true;
                await AllClear();
                var result = await _http.Get(new AiurUrl(_configuration["ResourcesUrl"] + "structure.json"), false);
                var sourceObject = JsonConvert.DeserializeObject<List<Collection>>(result);

                // Get all collections
                foreach (var collection in sourceObject)
                {
                    // Insert collection
                    var newCollection = new Collection
                    {
                        CollectionTitle = collection.CollectionTitle
                    };
                    _dbContext.Collections.Add(newCollection);
                    await _dbContext.SaveChangesAsync();

                    // Get markdown from GitHub
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

                    // Generate markdown from doc generator
                    if (!string.IsNullOrWhiteSpace(collection.DocAPIAddress))
                    {
                        var docString = await _http.Get(new AiurUrl(collection.DocAPIAddress), false);
                        var docModel = JsonConvert.DeserializeObject<List<API>>(docString);
                        var docGrouped = docModel.GroupBy(t => t.ControllerName);
                        var apiRoot = collection.DocAPIAddress.ToLower().Replace("/doc", "");
                        foreach (var docController in docGrouped)
                        {
                            var markdown = _markdown.GenerateMarkDownForAPI(docController, apiRoot);
                            var newarticle = new Article
                            {
                                ArticleTitle = docController.Key.TrimController(),
                                ArticleContent = markdown,
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

    public class MarkDownGenerator
    {
        private string _post = "<span class=\"badge badge-pill badge-warning text-white\">POST</span>";
        private string _get = "<span class=\"badge badge-pill badge-success\">GET</span>";
        private string _authorized = "<span class=\"badge badge-pill badge-danger\">Authorize</span>";
        private string _unauthorized = "<span class=\"badge badge-pill badge-secondary\">Anonymous</span>";

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

        public string GenerateMarkDownForAPI(IGrouping<string, API> docController, string apiRoot)
        {
            var content = $"# {docController.Key.TrimController()}\r\n\r\n";
            content += $"## Catalog\r\n\r\n";
            foreach (var docAction in docController)
            {
                content += $"* [{docAction.ActionName.SplitStringUpperCase()}](#{docAction.ActionName})\r\n";
            }
            content += $"\r\n";
            foreach (var docAction in docController)
            {
                content += $"<h3 id='{docAction.ActionName}'>{(docAction.IsPost ? _post : _get)} {(docAction.AuthRequired ? _authorized : string.Empty)} {docAction.ActionName.SplitStringUpperCase()}</h3>\r\n\r\n";
                content += $"Request path:\r\n\r\n";
                content += $"\t{apiRoot}/{docAction.ControllerName.TrimController()}/{docAction.ActionName}\r\n\r\n";
                if (docAction.IsPost)
                {
                    content += $"Request content type:\r\n\r\n";
                    content += docAction.RequiresFile ? "\tmultipart/form-data\r\n\r\n" : "\tapplication/x-www-form-urlencoded\r\n\r\n";
                }
                if (docAction.Arguments.Count > 0)
                {
                    content += $"Request {(docAction.IsPost ? "form" : "arguments")}:\r\n\r\n";
                    content += $"| Name | Required | Type |\r\n";
                    content += $"|----------|:-------------:|:------:|\r\n";
                    foreach (var arg in docAction.Arguments)
                    {
                        content += $"|{arg.Name}|{(arg.Required ? "<b class='text-danger'>Required</b>" : "Not required")}|<b class='text-danger'>{(ArgTypeConverter(arg.Type))}</b>|\r\n";
                    }
                    if (docAction.RequiresFile)
                    {
                        content += $"|File|<b class='text-danger'>Required</b>|<b class='text-danger'>File</b>|\r\n";
                    }
                    content += $"\r\n";
                }
            }
            return content;
        }
    }
}
