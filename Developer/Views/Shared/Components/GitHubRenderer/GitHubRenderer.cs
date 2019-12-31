using Aiursoft.Pylon.Services;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Views.Shared.Components.GitHubRenderer
{
    public class GitHubRenderer : ViewComponent
    {
        private readonly HTTPService _http;
        private readonly AiurCache _cache;

        public GitHubRenderer(
            HTTPService http,
            AiurCache cache)
        {
            _http = http;
            _cache = cache;
        }

        public async Task<IViewComponentResult> InvokeAsync(string org, string repo)
        {
            var markdownUrl = $"https://raw.githubusercontent.com/{org}/{repo}/master/README.md";
            var markdown = await _cache.GetAndCache($"github.{org}.{repo}.cache",
                async () => await _http.Get(new AiurUrl(markdownUrl), false));
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();
            var html = Markdown.ToHtml(markdown, pipeline);
            var model = new GitHubRendererViewModel
            {
                Org = org,
                Repo = repo,
                HTML = html
            };
            return View(model);
        }
    }
}
