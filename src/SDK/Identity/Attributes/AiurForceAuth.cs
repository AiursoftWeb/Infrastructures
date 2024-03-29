﻿using Aiursoft.Directory.SDK.Configuration;
using Aiursoft.Directory.SDK.Services;
using Aiursoft.SDK.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Aiursoft.Identity.Attributes;

/// <summary>
///     Request the signed in token or throw a NotAiurSignedInException
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public class AiurForceAuth : ActionFilterAttribute, IAiurForceAuth
{
    public AiurForceAuth(bool directlyReject = false)
    {
        _directlyReject = directlyReject;
    }

    public AiurForceAuth(string preferController, string preferAction, bool justTry, bool register = false)
    {
        _preferController = preferController;
        _preferAction = preferAction;
        _justTry = justTry ? true : null;
        _preferPageSet = true;
        _register = register;
    }

    private string _preferController { get; }
    private string _preferAction { get; }
    private bool? _justTry { get; } = false;
    private bool _preferPageSet { get; }
    private bool _register { get; }
    private bool _directlyReject { get; }

    private bool _hasAPreferPage => (!string.IsNullOrEmpty(_preferController)
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

            return $"/{_preferController}/{_preferAction}";
        }
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);
        if (context.Controller is not ControllerBase controller)
            // If goes here, it seems we are not using it on a controller.
        {
            throw new InvalidOperationException();
        }

        //Not signed in
        if (!controller.User.Identity?.IsAuthenticated ?? false)
        {
            if (_hasAPreferPage)
            {
                // Just redirected back, leave him here.
                var show = context.HttpContext.Request.Query[AuthValues.DirectShowString.Key];
                if (_justTry == true && show == AuthValues.DirectShowString.Value)
                {
                    return;
                }

                // Try him.
                context.Result = Redirect(context, _preferPage, _justTry, _register);
            }
            // Directly response a 403
            else if (_directlyReject)
            {
                context.Result = new UnauthorizedResult();
            }
            // Don't have a prefer page, force him to sign in.
            else
            {
                context.Result = Redirect(context, controller.Request.Path.Value, null, _register);
            }
        }
        //Signed in, let him go to preferred page directly.
        else if (_hasAPreferPage &&
                 !(controller.Request.Path.Value?.ToLower().StartsWith(_preferPage.ToLower()) ?? true))
        {
            context.HttpContext.Response.Redirect(_preferPage);
        }
        //Signed in and no preferred page, Display current page.
    }

    private RedirectResult Redirect(ActionExecutingContext context, string page, bool? justTry, bool register)
    {
        var urlConverter = context.HttpContext.RequestServices.GetRequiredService<UrlConverter>();
        var configuration = context.HttpContext.RequestServices.GetRequiredService<IOptions<DirectoryConfiguration>>();
        var serverPosition = $"{context.HttpContext.Request.Scheme}://{context.HttpContext.Request.Host}";
        var url = urlConverter.UrlWithAuth(serverPosition, configuration.Value.AppId, page, justTry, register);
        return new RedirectResult(url);
    }
}