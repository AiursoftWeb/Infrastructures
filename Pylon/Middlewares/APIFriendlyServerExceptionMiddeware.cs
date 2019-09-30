using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Middlewares
{
    public class APIFriendlyServerExceptionMiddeware
    {
        private readonly RequestDelegate _next;
        private readonly ServiceLocation _serviceLocation;
        private readonly DiagnosticSource _diagnosticSource;

        public APIFriendlyServerExceptionMiddeware(
            RequestDelegate next,
            ServiceLocation serviceLocation,
            DiagnosticSource diagnosticSource)
        {
            _next = next;
            _serviceLocation = serviceLocation;
            _diagnosticSource = diagnosticSource;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception e)
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.Clear();
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json; charset=utf-8";
                    var projectName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
                    var message = JsonConvert.SerializeObject(new AiurProtocol
                    {
                        Code = ErrorType.UnknownError,
                        Message = $"{projectName} server was crashed! Sorry about that."
                    });
                    await context.Response.WriteAsync(message, Encoding.UTF8);

                    if (_diagnosticSource.IsEnabled("Microsoft.AspNetCore.Diagnostics.UnhandledException"))
                    {
                        _diagnosticSource.Write("Microsoft.AspNetCore.Diagnostics.UnhandledException", new { httpContext = context, exception = e });
                    }
                    return;
                }
                throw;
            }
        }
    }
}
