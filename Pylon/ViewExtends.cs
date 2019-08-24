using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Aiursoft.Pylon.Services;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Html;

namespace Aiursoft.Pylon
{
    public static class ViewExtends
    {
        private static IHtmlContentBuilder AppendJavaScript(this IHtmlContentBuilder content, string path)
        {
            return content.AppendHtml($"\n<script src='{path}'></script>");
        }

        private static IHtmlContentBuilder AppendStyleSheet(this IHtmlContentBuilder content, string path)
        {
            return content.AppendHtml($"\n<link href='{path}' rel='stylesheet' />");
        }

        public static IHtmlContent UseAiurDashboardCss(this RazorPage page, bool includeCore = true)
        {
            var serviceLocation = page.Context.RequestServices.GetService<ServiceLocation>();
            if (includeCore)
            {
                return new HtmlContentBuilder()
                    .AppendStyleSheet($"{serviceLocation.UI}/dist/AiurCore.min.css")
                    .AppendStyleSheet($"{serviceLocation.UI}/dist/AiurDashboard.min.css");
            }
            else
            {
                return new HtmlContentBuilder()
                    .AppendStyleSheet($"{serviceLocation.UI}/dist/AiurDashboard.min.css");
            }
        }

        public static IHtmlContent UseAiurDashboardJs(this RazorPage page, bool includeCore = true)
        {
            var serviceLocation = page.Context.RequestServices.GetService<ServiceLocation>();
            if (includeCore)
            {
                return new HtmlContentBuilder()
                    .AppendJavaScript($"{serviceLocation.UI}/dist/AiurCore.min.js")
                    .AppendJavaScript($"{serviceLocation.UI}/dist/AiurDashboard.min.js");
            }
            else
            {
                return new HtmlContentBuilder()
                    .AppendJavaScript($"{serviceLocation.UI}/dist/AiurDashboard.min.js");
            }
        }

        public static IHtmlContent UseAiurMarketCss(this RazorPage page)
        {
            var serviceLocation = page.Context.RequestServices.GetService<ServiceLocation>();
            return new HtmlContentBuilder()
                .AppendStyleSheet($"{serviceLocation.UI}/dist/AiurCore.min.css")
                .AppendStyleSheet($"{serviceLocation.UI}/dist/AiurMarket.min.css");
        }

        public static IHtmlContent UseAiurMarketJs(this RazorPage page)
        {
            var serviceLocation = page.Context.RequestServices.GetService<ServiceLocation>();
            return new HtmlContentBuilder()
                .AppendJavaScript($"{serviceLocation.UI}/dist/AiurCore.min.js")
                .AppendJavaScript($"{serviceLocation.UI}/dist/AiurMarket.min.js");
        }

        public static IHtmlContent UseAiurFavicon(this RazorPage page)
        {
            var serviceLocation = page.Context.RequestServices.GetService<ServiceLocation>();
            return new HtmlContentBuilder()
                .SetHtmlContent($"<link rel='icon' type='image/x-icon' href='{serviceLocation.UI}/favicon.ico'>");
        }

        public static IHtmlContent UseDnsPrefetch(this RazorPage page)
        {
            var serviceLocation = page.Context.RequestServices.GetService<ServiceLocation>();
            var builder = new HtmlContentBuilder();
            string[] domains = {
                serviceLocation.API,
                serviceLocation.OSSEndpoint,
                serviceLocation.UI,
                serviceLocation.Account
            };
            foreach (var domain in domains)
            {
                builder.AppendHtml($"\n<link rel='dns-prefetch' href='{domain}'>");
            }
            return builder;
        }

        public static IHtmlContent UseSEO(this RazorPage page)
        {
            var description = page.ViewBag.Des as string;
            if(string.IsNullOrWhiteSpace(description))
            {
                description = "Create a more open world. Aiursoft is focusing on open platform and open communication. Free training, tools, and community to help you grow your skills, career, or business.";
            }
            return new HtmlContentBuilder()
                .SetHtmlContent($"<meta name=\"description\" content=\"{description}\" />");
        }

        public static IHtmlContent UseDisableZoom(this RazorPage page)
        {
            return new HtmlContentBuilder().SetHtmlContent("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1, maximum-scale=1, user-scalable=0\">");
        }
    }
}
