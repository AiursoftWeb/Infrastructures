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

namespace Aiursoft.Wiki.Services
{
    public class Seeder
    {
        public bool Seeding { get; set; } = false;
        public WikiDbContext _dbContext { get; set; }
        private readonly IConfiguration _configuration;

        public Seeder(
            WikiDbContext dbContext,
            IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }
        
        public async Task Seed()
        {
            try
            {
                Seeding = true;
                _dbContext.Article.Delete(t => true);
                _dbContext.Collections.Delete(t => true);
                await _dbContext.SaveChangesAsync();
                var http = new HTTPService();
                var result = await http.Get(new AiurUrl(_configuration["ResourcesUrl"] + "structure.json"));
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
                            ArticleContent = await http.Get(new AiurUrl($"{_configuration["ResourcesUrl"]}{collection.CollectionTitle}/{article.ArticleTitle}.md")),
                            CollectionId = newCollection.CollectionId
                        };
                        _dbContext.Article.Add(newarticle);
                        await _dbContext.SaveChangesAsync();
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
