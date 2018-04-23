using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Aiursoft.Wiki.Models;
using Aiursoft.Wiki.Models.HomeViewModels;
using Aiursoft.Wiki.Data;
using Microsoft.EntityFrameworkCore;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Models;
using Markdig;
using Aiursoft.Pylon.Services;
using Newtonsoft.Json;
using Aiursoft.Wiki.Services;

namespace Aiursoft.Wiki.Controllers
{
    public class HomeController : Controller
    {
        public readonly SignInManager<WikiUser> _signInManager;
        public readonly ILogger _logger;
        public readonly WikiDbContext _dbContext;
        public readonly Seeder _seeder;
        public readonly ServiceLocation _serviceLocation;
        public HomeController(
            SignInManager<WikiUser> signInManager,
            ILoggerFactory loggerFactory,
            WikiDbContext _context,
            Seeder seeder,
            ServiceLocation serviceLocation)
        {
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<HomeController>();
            this._dbContext = _context;
            _seeder = seeder;
            _serviceLocation = serviceLocation;
        }
        [AiurForceAuth(preferController: "Home", preferAction: "Index", justTry: true)]
        public async Task<IActionResult> Index()//Title
        {
            var firstArticle = await _dbContext.Article.Include(t => t.Collection).FirstAsync();
            return Redirect($"/ReadDoc/{firstArticle.Collection.CollectionTitle}/{firstArticle.ArticleTitle}.md");
        }

        [Route(template: "/ReadDoc/{CollectionTitle}/{ArticleTitle}.md")]
        public async Task<IActionResult> _ReadDoc(string CollectionTitle, string ArticleTitle)
        {
            var database = await _dbContext.Collections.Include(t => t.Articles).ToListAsync();
            var currentCollection = database.SingleOrDefault(t => t.CollectionTitle.ToLower() == CollectionTitle.ToLower());
            if (currentCollection == null)
            {
                return NotFound();
            }
            var currentArticle = currentCollection.Articles.SingleOrDefault(t => t.ArticleTitle.ToLower() == ArticleTitle.ToLower());
            if (currentArticle == null)
            {
                return NotFound();
            }
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();
            var model = new WikiViewModel
            {
                Collections = database,
                CurrentCollection = currentCollection,
                CurrentArticle = currentArticle,
                RenderedContent = Markdown.ToHtml(currentArticle.ArticleContent, pipeline)
            };
            return View(model);
        }
        public async Task<IActionResult> ToJson()
        {
            var database = await _dbContext.Collections.Include(t => t.Articles).ToListAsync();
            return Json(database);
        }
        [HttpPost]
        public async Task<IActionResult> Seed()
        {
            if (_seeder.Seeding)
            {
                return this.Protocal(ErrorType.Pending, $"Seeding...");
            }
            await _seeder.Seed();
            return Json(new AiurProtocal
            {
                Code = ErrorType.Success,
                Message = "Seeded"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return this.SignoutRootServer(_serviceLocation.API, new AiurUrl(string.Empty, "Home", nameof(HomeController.Index), new { }));
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
