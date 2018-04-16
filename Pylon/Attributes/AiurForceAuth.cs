using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Aiursoft.Pylon.Attributes
{
    /// <summary>
    /// Request the signed in token or throw a NotAiurSignedInException
    /// </summary>
    public class AiurForceAuth : ActionFilterAttribute
    {
        private string _preferController { get; set; } = null;
        private string _preferAction { get; set; } = null;
        private bool? _justTry { get; set; } = false;
        private bool _preferPageSet { get; set; } = false;
        private bool _register { get; set; } = false;

        private bool _hasAPreferPage => (true
            && !string.IsNullOrEmpty(_preferController)
            && !string.IsNullOrEmpty(_preferAction))
            || _preferPageSet;

        private string _preferPage
        {
            get
            {
                if (string.IsNullOrEmpty(_preferController) && string.IsNullOrEmpty(_preferAction))
                {
                    return "/";
                }
                return new AiurUrl(string.Empty, _preferController, _preferAction, new { }).ToString();
            }
        }

        public AiurForceAuth()
        {

        }

        public AiurForceAuth(string preferController, string preferAction, bool justTry, bool register = false)
        {
            _preferController = preferController;
            _preferAction = preferAction;
            _justTry = justTry ? true as bool? : null;
            _preferPageSet = true;
            _register = register;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            var controller = context.Controller as Controller;
            var show = context.HttpContext.Request.Query[Values.DirectShowString.Key];
            //Not signed in
            if (!controller.User.Identity.IsAuthenticated)
            {
                if (_hasAPreferPage)
                {
                    // Just redirected back.
                    if (show == Values.DirectShowString.Value && _justTry == true)
                    {
                        return;
                    }
                    // Try him.
                    context.Result = _Redirect(context, _preferPage, _justTry, _register);
                }
                else
                {
                    context.Result = _Redirect(context, controller.Request.Path.Value, justTry: null, register: _register);
                }
            }
            //Signed in, let him go to prefered page directly.
            else if (_hasAPreferPage && !controller.Request.Path.Value.ToLower().StartsWith(_preferPage.ToLower()))
            {
                context.HttpContext.Response.Redirect(_preferPage);
            }
            //Signed in and no prefered page, Display current page.
            else
            {
                return;
            }
        }

        private RedirectResult _Redirect(ActionExecutingContext context, string page, bool? justTry, bool register)
        {
            var r = context.HttpContext.Request;
            string serverPosition = $"{r.Scheme}://{r.Host}";
            string url = UrlConverter.UrlWithAuth(serverPosition, page, justTry, register);
            return new RedirectResult(url);
        }
    }
}
