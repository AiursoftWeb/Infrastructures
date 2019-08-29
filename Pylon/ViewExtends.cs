using Aiursoft.Pylon.Services;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;

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
    }
}
