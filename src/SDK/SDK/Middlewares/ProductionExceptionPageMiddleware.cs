using System;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Aiursoft.Directory.SDK.Services;
using Aiursoft.Handler.Models;
using Aiursoft.Observer.SDK.Services.ToObserverServer;
using Aiursoft.SDK.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Aiursoft.SDK.Middlewares;

public class ProductionExceptionPageMiddleware
{
    private readonly ServiceLocation _serviceLocation;
    private readonly RequestDelegate _next;

    public ProductionExceptionPageMiddleware(
        ServiceLocation serviceLocation,
        RequestDelegate next)
    {
        _serviceLocation = serviceLocation;
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch
        {
            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            if (IsAccessingAPI(context))
            {
                context.Response.ContentType = "application/json; charset=utf-8";
                var projectName = Assembly.GetEntryAssembly()?.GetName().Name;
                var message = JsonConvert.SerializeObject(new AiurProtocol
                {
                    Code = ErrorType.UnknownError,
                    Message = $"{projectName} server was crashed! Sorry about that."
                });
                await context.Response.WriteAsync(message, Encoding.UTF8);
            }
            else
            {
                var text = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
<meta charset=""utf-8"">
<meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
<meta name=""viewport"" content=""width=device-width, initial-scale=1"">
<title>500 Server Exception - Aiursoft</title>
<link rel=""icon"" type=""image/x-icon"" href=""{_serviceLocation.UI}/favicon.ico"">
<link href=""{_serviceLocation.UI}/dist/AiurDashboard.min.css"" rel=""stylesheet"">
</head>
<body class=""bg-light lines"">
<div class=""container"">
<div class=""card card-register mx-auto mt-5"">
<div class=""card-header"">500 Server Exception</div>
<div class=""card-body"">
<h1 class=""text-danger"">500 - Server Exception</h1>
<h3>This operation caused an error on our server.</h3>
<p>Sorry about it. Please try again and tell us about the problem.</p>
<p>Or you can <a href=""{_serviceLocation.Status}"" target=""_blank"">view service health status</a> or submit an issue to our team.</p>
</div>
<a class=""card-footer clearfix small z-1"" href=""{_serviceLocation.WWW}"">
<span class=""float-left"">Back to Home</span>
<span class=""float-right""><i class=""fa fa-angle-right""></i></span>
</a>
</div>
</div>
<script src=""{_serviceLocation.UI}/dist/AiurDashboard.min.js""></script>
</body>
</html>";
                await context.Response.WriteAsync(text, Encoding.UTF8);
            }
        }
    }

    private bool IsAccessingAPI(HttpContext context)
    {
        var actionDescriptor = context.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>();

        var controllerType = actionDescriptor?.ControllerTypeInfo;

        return controllerType != null && controllerType.BaseType == typeof(ControllerBase);
    }
}