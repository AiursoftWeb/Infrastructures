using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Attributes
{
    public class APIExpHandler : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);
            switch (context.Exception)
            {
                case AiurUnexceptedResponse exp:
                    var arg = new AiurProtocal
                    {
                        Code = exp.Response.Code,
                        Message = exp.Response.Message
                    };
                    context.ExceptionHandled = true;
                    context.Result = new JsonResult(arg);
                    break;
                case AiurAPIModelException exp:
                    context.ExceptionHandled = true;
                    context.Result = new JsonResult(new AiurProtocal { Code = exp.Code, Message = exp.Message });
                    break;
            }
        }
    }
}
