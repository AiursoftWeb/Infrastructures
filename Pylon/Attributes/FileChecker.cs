using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace Aiursoft.Pylon.Attributes
{
    /// <summary>
    /// Will check if user is submitting a valid file. If not, will change the ModalState.IsValid to false and add a model error.
    /// </summary>
    public class FileChecker : ActionFilterAttribute
    {
        /// <summary>
        /// Of b. For example: 30 x 1024 x 1024 = 30MB.
        /// </summary>
        public long MaxSize { get; set; } = int.MaxValue;
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
                if (file.Length > MaxSize)
                {
                    context.ModelState.AddModelError("", "Please provide a file which is smaller than 1GB!");
                }
            }
            catch (Exception e)
            {
                context.ModelState.AddModelError("", e.Message);
            }
        }
    }
}
