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
            var content = new HtmlContentBuilder();
            content.SetHtmlContent($"<link href='{serviceLocation.CDN}/dist/AiurCore.min.css' rel='stylesheet'><link href='{serviceLocation.CDN}/dist/AiurDashboard.min.css' rel='stylesheet'>");
            return content;
        }

        public static IHtmlContent UseAiurDashboardJs(this RazorPage page)
        {
            var serviceLocation = page.Context.RequestServices.GetService<ServiceLocation>();
            var content = new HtmlContentBuilder();
            content.SetHtmlContent($"<script src='{serviceLocation.CDN}/dist/AiurCore.min.js'></script><script src='{serviceLocation.CDN}/dist/AiurDashboard.min.js'></script>");
            return content;
        }
    }
}
