using Aiursoft.Pylon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aiursoft.Pylon.Attributes
{
    public class FileChecker : ActionFilterAttribute
    {
        public long MaxSize { get; set; } = -1;
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            try
            {
                // Not a post method
                if (context.HttpContext.Request.Method.ToUpper().Trim() != "POST")
                {
                    context.ModelState.AddModelError("", "To upload your file, you have to submit the form!");
                    return;
                }
                // No file
                if (context.HttpContext.Request.Form.Files.Count < 1)
                {
                    context.ModelState.AddModelError("", "Please provide a file!");
                    return;
                }
                var file = context.HttpContext.Request.Form.Files.First();
                // File is null
                if (file == null)
                {
                    context.ModelState.AddModelError("", "Please provide a file!");
                    return;
                }
                // Too small
                if (file.Length < 1)
                {
                    context.ModelState.AddModelError("", "Please provide a valid file!");
                    return;
                }
                // Too large
                if ((MaxSize != -1 && file.Length > MaxSize) || file.Length > Values.MaxFileSize)
                {
                    context.ModelState.AddModelError("", "Please provide a file which is smaller than 1GB!");
                    return;
                }
            }
            catch (Exception e)
            {
                context.ModelState.AddModelError("", e.Message);
                return;
            }
        }
    }
}
