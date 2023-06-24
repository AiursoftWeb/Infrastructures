using System;
using System.Collections.Generic;
using System.Linq;
using Aiursoft.DocGenerator.Middlewares;
using Aiursoft.DocGenerator.Tools;
using Aiursoft.Scanner.Abstraction;
using Newtonsoft.Json;

namespace Aiursoft.DocGenerator.Services;

public class MarkDownDocGenerator : ITransientDependency
{
    private const string Post = "<span class=\"badge badge-pill badge-warning text-white\">POST</span>";
    private const string Get = "<span class=\"badge badge-pill badge-success\">GET</span>";

    private const string Authorized =
        "<span class=\"badge badge-pill badge-danger\"><i class=\"fa fa-shield\"></i> Authorize</span>";

    private string ArgTypeConverter(ArgumentType type)
    {
        return type switch
        {
            ArgumentType.Boolean => "Boolean",
            ArgumentType.Text => "Text",
            ArgumentType.Number => "Number",
            ArgumentType.Datetime => "DateTime",
            ArgumentType.Collection => "Text Collection",
            ArgumentType.Unknown => "A magic type!",
            _ => throw new InvalidOperationException(type.ToString())
        };
    }

    private string GetExampleValue(Argument arg)
    {
        return arg.Type switch
        {
            ArgumentType.Boolean => "false",
            ArgumentType.Datetime => "01/01/2018",
            ArgumentType.Number => "0",
            ArgumentType.Text => $"your{arg.Name}",
            ArgumentType.Collection => $"your{arg.Name}",
            ArgumentType.Unknown => $"your{arg.Name}",
            _ => $"your{arg.Name}"
        };
    }

    private string GenerateQueryParams(List<Argument> args)
    {
        var path = "";
        foreach (var arg in args)
        {
            if (arg.Type != ArgumentType.Collection)
            {
                path += $"{arg.Name}={GetExampleValue(arg)}&";
            }
            else
            {
                path +=
                    $"{arg.Name}[0]={GetExampleValue(arg)}&{arg.Name}[1]={GetExampleValue(arg)}&{arg.Name}[2]={GetExampleValue(arg)}&";
            }
        }

        return path.Trim('&');
    }

    private bool RouteContainsArg(string routeTemplate, Argument arg)
    {
        return
            routeTemplate.ToLower().Contains("{" + arg.Name.ToLower() + "}") ||
            routeTemplate.ToLower().Contains("{**" + arg.Name.ToLower() + "}");
    }

    private string GenerateRequestPathExample(string routeTemplate, List<Argument> args)
    {
        foreach (var arg in args)
        {
            if (!RouteContainsArg(routeTemplate, arg))
            {
                continue;
            }

            routeTemplate = routeTemplate.ToLower()
                .Replace("{" + arg.Name.ToLower() + "}", "{" + GetExampleValue(arg) + "}");
            routeTemplate = routeTemplate.ToLower()
                .Replace("{**" + arg.Name.ToLower() + "}", "{" + GetExampleValue(arg) + "}");
        }

        return routeTemplate;
    }

    public string GenerateMarkDownForApi(IGrouping<string, API> docController, string apiRoot)
    {
        var content = $"# {docController.Key.TrimController()}\r\n\r\n";
        content += "## Catalog\r\n\r\n";
        foreach (var docAction in docController)
        {
            content += $"* [{docAction.ActionName.SplitStringUpperCase()}](#{docAction.ActionName})\r\n";
        }

        content += "\r\n";
        foreach (var docAction in docController)
        {
            var queryParams = docAction.Arguments
                .Where(argument => !docAction.Routes.Any(route => RouteContainsArg(route, argument))).ToList();
            content += "---------\r\n\r\n";
            content +=
                $"<h3 id='{docAction.ActionName}'>{(docAction.IsPost ? Post : Get)} {(docAction.AuthRequired ? Authorized : string.Empty)} {docAction.ActionName.SplitStringUpperCase()}</h3>\r\n\r\n";
            // Request path.
            foreach (var route in docAction.Routes)
            {
                var path = $"{apiRoot}/{route}";
                content += "Request path:\r\n\r\n";
                content += $"<kbd>{path}</kbd>";
                content +=
                    $"<button class=\"btn btn-sm btn-secondary ml-1\" href=\"#\" data-toggle=\"tooltip\" data-trigger=\"click\" title=\"copied!\" data-clipboard-text=\"{path}\">Copy</button>";
                content += "\r\n\r\n";

                var pathWithArgs =
                    $"{apiRoot}/{GenerateRequestPathExample(route, docAction.Arguments)}?{GenerateQueryParams(queryParams)}"
                        .TrimEnd('?');
                // Path Example.
                content += "Request example:\r\n\r\n";
                content += $"\t{pathWithArgs}\r\n\r\n";
                if (docAction.IsPost)
                {
                    continue;
                }

                // Add the try button.
                content +=
                    $"<a class=\"btn btn-sm btn-primary ml-1\" target=\"_blank\" href=\"{pathWithArgs}\">Try</a>";
                content += "\r\n\r\n";
            }

            if (docAction.IsPost)
            {
                // POST Example
                content += "\r\n\r\n";
                content += "Request content type:\r\n\r\n";
                content += "\tapplication/x-www-form-urlencoded\r\n\r\n";

                content += "Form content example:\r\n\r\n";
                content += "```\r\n";
                content += $"{GenerateQueryParams(queryParams)} \r\n";
                content += "```\r\n\r\n";
            }

            if (docAction.Arguments.Count > 0)
            {
                content += $"Request {(docAction.IsPost ? "form" : "arguments")}:\r\n\r\n";
                content += "| Name | Required | Type |\r\n";
                content += "|----------|:-------------:|:------:|\r\n";
                foreach (var arg in docAction.Arguments)
                {
                    content +=
                        $"|{arg.Name}|{(arg.Required ? "<b class='text-danger'>Required</b>" : "Not required")}|<b class='text-primary'>{ArgTypeConverter(arg.Type)}</b>|\r\n";
                }

                content += "\r\n";
            }

            foreach (var possibleResponse in docAction.PossibleResponses)
            {
                content += "Possible Response:\r\n";
                var dybject = JsonConvert.DeserializeObject(possibleResponse);
                var finalResult = JsonConvert.SerializeObject(dybject, Formatting.Indented);
                content += "\r\n";
                content += "```json\r\n";
                content += finalResult;
                content += "\r\n```\r\n";
            }

            content += "\r\n";
        }

        return content;
    }
}