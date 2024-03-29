﻿using Aiursoft.Directory.SDK.Configuration;
using Aiursoft.Identity;
using Aiursoft.Wiki.Data;
using Aiursoft.Wiki.Models;
using Aiursoft.CSTools.Tools;
using Markdig;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Aiursoft.Wiki.Controllers;


public class HomeController : Controller
{
    private readonly WikiDbContext _dbContext;
    private readonly DirectoryConfiguration _directoryLocator;
    private readonly SignInManager<WikiUser> _signInManager;

    public HomeController(
        SignInManager<WikiUser> signInManager,
        WikiDbContext context,
        IOptions<DirectoryConfiguration> serviceLocation)
    {
        _signInManager = signInManager;
        _dbContext = context;
        _directoryLocator = serviceLocation.Value;
    }

    public async Task<IActionResult> Index() //Title
    {
        var firstArticle = await _dbContext.Article.Include(t => t.Collection).FirstOrDefaultAsync();
        if (firstArticle == null)
        {
            return NotFound();
        }

        return Redirect($"/{firstArticle.Collection.CollectionTitle}/{firstArticle.ArticleTitle}.md");
    }

    [Route("/{collectionTitle}/{articleTitle}.md")]
    public async Task<IActionResult> ReadDoc(string collectionTitle, string articleTitle)
    {
        var database = await _dbContext.Collections.Include(t => t.Articles).ToListAsync();
        var currentCollection = database.SingleOrDefault(t => t.CollectionTitle.ToLower() == collectionTitle.ToLower());
        if (currentCollection == null)
        {
            return NotFound();
        }

        var currentArticle =
            currentCollection.Articles.SingleOrDefault(t => t.ArticleTitle.ToLower() == articleTitle.ToLower());
        if (currentArticle == null)
        {
            return NotFound();
        }

        var pipeline = new MarkdownPipelineBuilder()
            .UseAbbreviations()
            .UseAutoIdentifiers()
            .UseCitations()
            .UseCustomContainers()
            .UseDefinitionLists()
            .UseEmphasisExtras()
            .UseFigures()
            .UseFooters()
            .UseFootnotes()
            .UseGridTables()
            .UseMathematics()
            .UseMediaLinks()
            .UsePipeTables()
            .UseListExtras()
            .UseTaskLists()
            .UseDiagrams()
            .UseAutoLinks()
            .Build();
        var model = new WikiViewModel
        {
            Collections = database,
            CurrentCollection = currentCollection,
            CurrentArticle = currentArticle,
            RenderedContent = Markdown.ToHtml(currentArticle.ArticleContent, pipeline)
        };
        ViewBag.Des = Markdown.ToPlainText(currentArticle.ArticleContent).SafeSubstring(1000);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> LogOff()
    {
        await _signInManager.SignOutAsync();
        return this.SignOutRootServer(_directoryLocator.Instance, $"Home/{nameof(Index)}");
    }

    public IActionResult Error()
    {
        return View();
    }
}