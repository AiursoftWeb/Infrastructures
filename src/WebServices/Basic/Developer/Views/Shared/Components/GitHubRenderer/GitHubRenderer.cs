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

        public async Task<IViewComponentResult> InvokeAsync(string org, string repo, string path)
        {
            var markdownUrl = $"https://raw.githubusercontent.com/{org}/{repo}/master/{path}";
            var markdown = await _cache.GetAndCache($"github.{org}.{repo}.{path}.cache",
                async () => await _http.Get(new AiurUrl(markdownUrl)));
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();
            var html = Markdown.ToHtml(markdown, pipeline);
            var model = new GitHubRendererViewModel
            {
                Org = org,
                Repo = repo,
                HTML = html,
                Path = path
            };
            return View(model);
        }
    }
}
