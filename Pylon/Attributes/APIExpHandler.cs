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
    public class APIExpHandler: ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);
            switch (context.Exception)
            {
                case AiurUnexceptedResponse exp:
                    var arg = new AiurProtocal
                    {
                        code = exp.Response.code,
                        message = exp.Response.message
                    };
                    context.ExceptionHandled = true;
                    context.Result = new JsonResult(arg);
                    break;
            }
        }
    }
}
