using System;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Aiursoft.Pylon.Services;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Html;

namespace Aiursoft.Pylon
{
    public static class ViewExtends
    {
        public static IHtmlContentBuilder AppendJavaScript(this IHtmlContentBuilder content, string path)
        {
            return content.AppendHtmlLine($"<script src='{path}'></script>");
        }

        public static IHtmlContentBuilder AppendStyleSheet(this IHtmlContentBuilder content, string path)
        {
            return content.AppendHtmlLine($"<link href='{path}/dist/AiurCore.min.css' rel='stylesheet' />");
        }

        public static IHtmlContent UseAiurFooter()
        {
            throw new NotImplementedException();
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
            content.SetHtmlContent("<a class='scroll-to-top rounded' href='#page-top'><i class='fa fa-angle-up'></i></a>");
            return content;
        }

        public static IHtmlContent UseAiurDashboardCSS(this RazorPage page)
        {
            var serviceLocation = page.Context.RequestServices.GetService<ServiceLocation>();
            return new HtmlContentBuilder()
                .AppendStyleSheet($"{serviceLocation.CDN}/dist/AiurCore.min.css")
                .AppendStyleSheet($"{serviceLocation.CDN}/dist/AiurDashboard.min.css");
        }

        public static IHtmlContent UseAiurDashboardJs(this RazorPage page)
        {
            var serviceLocation = page.Context.RequestServices.GetService<ServiceLocation>();
            return new HtmlContentBuilder()
                .AppendJavaScript($"{serviceLocation.CDN}/dist/AiurCore.min.js")
                .AppendJavaScript($"{serviceLocation.CDN}/dist/AiurDashboard.min.js");
        }

        public static IHtmlContent UseAiurFavicon(this RazorPage page)
        {
            var serviceLocation = page.Context.RequestServices.GetService<ServiceLocation>();
            return new HtmlContentBuilder()
                .SetHtmlContent($"<link rel='icon' type='image/x-icon' href='{serviceLocation.CDN}/favicon.ico'>");
        }
    }
}
