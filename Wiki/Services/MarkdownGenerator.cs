using Aiursoft.Pylon.Middlewares;
using Aiursoft.Pylon.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Wiki.Services
{
    public class MarkDownGenerator
    {
        private string _post = "<span class=\"badge badge-pill badge-warning text-white\">POST</span>";
        private string _get = "<span class=\"badge badge-pill badge-success\">GET</span>";
        private string _authorized = "<span class=\"badge badge-pill badge-danger\"><i class=\"fa fa-shield\"></i> Authorize</span>";
        private string _unauthorized = "<span class=\"badge badge-pill badge-secondary\">Anonymous</span>";

        private string ArgTypeConverter(ArgumentType type)
        {
            switch (type)
            {
                case ArgumentType.boolean:
                    return "Boolean";
                case ArgumentType.text:
                    return "Text";
                case ArgumentType.number:
                    return "Number";
                case ArgumentType.datetime:
                    return "DateTime";
                case ArgumentType.unknown:
                    return "A magic type!";
            }
            throw new InvalidOperationException(type.ToString());
        }

        public string GenerateMarkDownForAPI(IGrouping<string, API> docController, string apiRoot)
        {
            var content = $"# {docController.Key.TrimController()}\r\n\r\n";
            content += $"## Catalog\r\n\r\n";
            foreach (var docAction in docController)
            {
                content += $"* [{docAction.ActionName.SplitStringUpperCase()}](#{docAction.ActionName})\r\n";
            }
            content += $"\r\n";
            foreach (var docAction in docController)
            {
                content += $"<h3 id='{docAction.ActionName}'>{(docAction.IsPost ? _post : _get)} {(docAction.AuthRequired ? _authorized : string.Empty)} {docAction.ActionName.SplitStringUpperCase()}</h3>\r\n\r\n";
                content += $"Request path:\r\n\r\n";
                content += $"\t{apiRoot}/{docAction.ControllerName.TrimController()}/{docAction.ActionName}\r\n\r\n";
                if (docAction.IsPost)
                {
                    content += $"Request content type:\r\n\r\n";
                    content += docAction.RequiresFile ? "\tmultipart/form-data\r\n\r\n" : "\tapplication/x-www-form-urlencoded\r\n\r\n";
                }
                if (docAction.Arguments.Count > 0)
                {
                    content += $"Request {(docAction.IsPost ? "form" : "arguments")}:\r\n\r\n";
                    content += $"| Name | Required | Type |\r\n";
                    content += $"|----------|:-------------:|:------:|\r\n";
                    foreach (var arg in docAction.Arguments)
                    {
                        content += $"|{arg.Name}|{(arg.Required ? "<b class='text-danger'>Required</b>" : "Not required")}|<b class='text-danger'>{(ArgTypeConverter(arg.Type))}</b>|\r\n";
                    }
                    if (docAction.RequiresFile)
                    {
                        content += $"|File|<b class='text-danger'>Required</b>|<b class='text-danger'>File</b>|\r\n";
                    }
                    content += $"\r\n";
                }
            }
            return content;
        }
    }
}
