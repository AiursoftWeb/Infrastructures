using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Attributes
{
    public class ProbeExceptionHandler : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);
            switch (context.Exception)
            {
                case AiurAPIModelException exp:
                    context.ExceptionHandled = true;
                    if (exp.Code == ErrorType.NotFound)
                    {
                        context.HttpContext.Response.StatusCode = 404;
                        context.Result = new JsonResult(new AiurProtocol { Code = exp.Code, Message = exp.Message });
                    }
                    break;
            }
        }
    }
}
