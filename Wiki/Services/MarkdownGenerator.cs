using Aiursoft.Pylon.Middlewares;
using Aiursoft.Pylon.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aiursoft.Wiki.Services
{
    public class MarkDownGenerator
    {
        private string _post = "<span class=\"badge badge-pill badge-warning text-white\">POST</span>";
        private string _get = "<span class=\"badge badge-pill badge-success\">GET</span>";
        private string _authorized = "<span class=\"badge badge-pill badge-danger\"><i class=\"fa fa-shield\"></i> Authorize</span>";

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
                case ArgumentType.collection:
                    return "Text Collection";
                case ArgumentType.unknown:
                    return "A magic type!";
            }
            throw new InvalidOperationException(type.ToString());
        }

        private string GetExampleValue(Argument arg)
        {
            switch (arg.Type)
            {
                case ArgumentType.boolean:
                    return "false";
                case ArgumentType.datetime:
                    return "01/01/2018";
                case ArgumentType.number:
                    return "0";
                case ArgumentType.text:
                case ArgumentType.collection:
                    return $"your{arg.Name}";
                default:
                    return $"your{arg.Name}";
            }
        }

        private string GenerateParams(List<Argument> args)
        {
            var path = "";
            foreach (var arg in args)
            {
                if (arg.Type != ArgumentType.collection)
                {
                    path += $"{arg.Name}={GetExampleValue(arg)}&";
                }
                else
                {
                    path += $"{arg.Name}[0]={GetExampleValue(arg)}&{arg.Name}[1]={GetExampleValue(arg)}&{arg.Name}[2]={GetExampleValue(arg)}&";
                }
            }
            return path.Trim('&');
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
                content += $"---------\r\n\r\n";
                content += $"<h3 id='{docAction.ActionName}'>{(docAction.IsPost ? _post : _get)} {(docAction.AuthRequired ? _authorized : string.Empty)} {docAction.ActionName.SplitStringUpperCase()}</h3>\r\n\r\n";
                content += $"Request path:\r\n\r\n";
                var path = $"{apiRoot}/{docAction.ControllerName.TrimController()}/{docAction.ActionName}";
                var pathWithArgs = $"{apiRoot}/{docAction.ControllerName.TrimController()}/{docAction.ActionName}?{GenerateParams(docAction.Arguments)}".TrimEnd('?');
                content += $"<kbd>{path}</kbd>";
                content += $"<button class=\"btn btn-sm btn-secondary ml-1\" href=\"#\" data-toggle=\"tooltip\" data-trigger=\"click\" title=\"copied!\" data-clipboard-text=\"{path}\">Copy</button>";
                if (docAction.IsPost == false)
                {
                    content += $"<a class=\"btn btn-sm btn-primary ml-1\" target=\"_blank\" href=\"{pathWithArgs}\">Try</a>";
                }
                content += "\r\n\r\n";
                if (docAction.IsPost == false)
                {
                    content += $"Request example:\r\n\r\n";
                    content += $"\t{pathWithArgs}\r\n\r\n";
                }
                if (docAction.IsPost)
                {
                    content += $"Request content type:\r\n\r\n";
                    content += "\tapplication/x-www-form-urlencoded\r\n\r\n";

                    content += $"Form content example:\r\n\r\n";
                    content += $"```\r\n";
                    content += $"{GenerateParams(docAction.Arguments)} \r\n";
                    content += $"```\r\n\r\n";
                }
                if (docAction.Arguments.Count > 0)
                {
                    content += $"Request {(docAction.IsPost ? "form" : "arguments")}:\r\n\r\n";
                    content += $"| Name | Required | Type |\r\n";
                    content += $"|----------|:-------------:|:------:|\r\n";
                    foreach (var arg in docAction.Arguments)
                    {
                        content += $"|{arg.Name}|{(arg.Required ? "<b class='text-danger'>Required</b>" : "Not required")}|<b class='text-primary'>{(ArgTypeConverter(arg.Type))}</b>|\r\n";
                    }
                    content += $"\r\n";
                }
                foreach (var possibleResponse in docAction.PossibleResponses)
                {
                    content += $"Possible Response:\r\n";
                    var dybject = JsonConvert.DeserializeObject(possibleResponse);
                    var finalresult = JsonConvert.SerializeObject(dybject, Formatting.Indented);
                    content += $"\r\n";
                    content += $"```json\r\n";
                    content += finalresult;
                    content += $"\r\n```\r\n";
                }
                content += $"\r\n";
            }
            return content;
        }
    }
}
