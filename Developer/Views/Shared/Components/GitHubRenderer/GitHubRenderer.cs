using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Views.Shared.Components.GitHubRenderer
{
    public class GitHubRenderer : ViewComponent
    {
        private readonly HTTPService _http;
        public GitHubRenderer(HTTPService http)
        {
            _http = http;
        }

        public async Task<IViewComponentResult> InvokeAsync(GitHubRendererArgs arg)
        {
            var markdownUrl = $"https://raw.githubusercontent.com/{arg.Org}/{arg.Repo}/master/README.md";
            var markdown = await _http.Get(new AiurUrl(markdownUrl), false);
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();
            var html = Markdown.ToHtml(markdown, pipeline);
            var model = new GitHubRendererViewModel
            {
                Org = arg.Org,
                Repo = arg.Repo,
                HTML = html
            };
            return View(model);
        }
    }
}
