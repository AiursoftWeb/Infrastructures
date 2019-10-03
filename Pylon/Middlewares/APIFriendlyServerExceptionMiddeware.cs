using Aiursoft.Pylon.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Middlewares
{
    public class APIFriendlyServerExceptionMiddeware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<APIFriendlyServerExceptionMiddeware> _logger;

        public APIFriendlyServerExceptionMiddeware(
            RequestDelegate next,
            ILogger<APIFriendlyServerExceptionMiddeware> logger)
        {
            _next = next;
            _logger = logger;
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
                    _logger.LogError(e, e.Message);
                    return;
                }
                throw;
            }
        }
    }
}
