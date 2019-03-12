using System;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.Pylon.Attributes
{
    /// <summary>
    /// Request the signed in token or throw a NotAiurSignedInException
    /// </summary>
    public class AiurForceAuth : ActionFilterAttribute
    {
        private string PreferController { get; }
        private string PreferAction { get; }
        private bool? JustTry { get; } = false;
        private bool PreferPageSet { get; }
        private bool Register { get; }
        private bool DirectlyReject { get; }

        private bool HasAPreferPage => (!string.IsNullOrEmpty(PreferController)
            && !string.IsNullOrEmpty(PreferAction))
            || PreferPageSet;

        private string PreferPage
        {
            get
            {
                if (string.IsNullOrEmpty(PreferController) && string.IsNullOrEmpty(PreferAction))
                {
                    return "/";
                }
                return new AiurUrl(string.Empty, PreferController, PreferAction, new { }).ToString();
            }
        }

        public AiurForceAuth(bool directlyReject = false)
        {
            DirectlyReject = directlyReject;
        }

        public AiurForceAuth(string preferController, string preferAction, bool justTry, bool register = false)
        {
            PreferController = preferController;
            PreferAction = preferAction;
            JustTry = justTry ? true : (bool?) null;
            PreferPageSet = true;
            Register = register;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (!(context.Controller is Controller controller))
            {
                // If goes here, it seems we are not using it on a controller.
                throw new InvalidOperationException();
            }
            var show = context.HttpContext.Request.Query[Values.DirectShowString.Key];
            //Not signed in
            if (!controller.User.Identity.IsAuthenticated)
            {
                if (HasAPreferPage)
                {
                    // Just redirected back, leave him here.
                    if (show == Values.DirectShowString.Value && JustTry == true)
                    {
                        return;
                    }
                    // Try him.
                    context.Result = Redirect(context, PreferPage, JustTry, Register);
                }
                // Directly response a 403
                else if (DirectlyReject)
                {
                    context.Result = new UnauthorizedResult();
                }
                // Don't have a prefer page, force him to sign in.
                else
                {
                    context.Result = Redirect(context, controller.Request.Path.Value, justTry: null, register: Register);
                }
            }
            //Signed in, let him go to preferred page directly.
            else if (HasAPreferPage && !controller.Request.Path.Value.ToLower().StartsWith(PreferPage.ToLower()))
            {
                context.HttpContext.Response.Redirect(PreferPage);
            }
            //Signed in and no preferred page, Display current page.
            else
            {
                return;
            }
        }

        private RedirectResult Redirect(ActionExecutingContext context, string page, bool? justTry, bool register)
        {
            var urlConverter = context.HttpContext.RequestServices.GetService<UrlConverter>();
            string serverPosition = $"{context.HttpContext.Request.Scheme}://{context.HttpContext.Request.Host}";
            string url = urlConverter.UrlWithAuth(serverPosition, page, justTry, register);
            return new RedirectResult(url);
        }
    }
}
