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
            return content.AppendHtmlLine($"<script src='{path}'></script>");
        }

        private static IHtmlContentBuilder AppendStyleSheet(this IHtmlContentBuilder content, string path)
        {
            return content.AppendHtmlLine($"<link href='{path}' rel='stylesheet' />");
        }

        public static IHtmlContent UseAiurLogoutter(this RazorPage page)
        {
            var template = @"<div class='modal fade' id='exampleModal' tabindex='-1' role='dialog' aria-labelledby='exampleModalLabel' aria-hidden='true'>
                                <div class='modal-dialog' role='document'>
                                    <form class='modal-content' action='/Home/Logoff' method='post'>
                                        <div class='modal-header'>
                                            <h5 class='modal-title' id='exampleModalLabel'>Ready to Leave?</h5>
                                            <button class='close' type='button' data-dismiss='modal' aria-label='Close'>
                                                <span aria-hidden='true'>×</span>
                                            </button>
                                        </div>
                                        <div class='modal-body'>Select 'Logout' below if you are ready to end your current session.</div>
                                        <div class='modal-footer'>
                                            <button class='btn btn-secondary' type='button' data-dismiss='modal'>Cancel</button>
                                            <input class='btn btn-primary' type='submit' value='Logout' />
                                        </div>
                                    </form>
                                </div>
                            </div>";
            return new HtmlContentBuilder()
                .SetHtmlContent(template);
        }

        public static IHtmlContent UseChinaRegisterInfo(this RazorPage page)
        {
            var content = new HtmlContentBuilder();
            var requestCultureFeature = page.Context.Features.Get<IRequestCultureFeature>();
            if (requestCultureFeature == null)
            {
                return content;
            }
            var requestCulture = requestCultureFeature.RequestCulture.UICulture.IetfLanguageTag;
            if (requestCulture == "zh")
            {
                content.SetHtmlContent("<a href='http://www.miitbeian.gov.cn' target='_blank'>辽ICP备17004979号-1</a>");
            }
            return content;
        }

        public static IHtmlContent UseScrollToTop(this RazorPage page)
        {
            var content = new HtmlContentBuilder();
            content.SetHtmlContent("<a class='aiur-scroll-to-top rounded' href='#page-top'><i class='fa fa-angle-up'></i></a>");
            return content;
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
                builder.AppendHtmlLine($@"<link rel='dns-prefetch' href='{domain}'>");
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
