using Aiursoft.DocGenerator.Attributes;
using Aiursoft.DocGenerator.Services;
using Aiursoft.DocGenerator.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Aiursoft.DocGenerator.Middlewares
{
    public class APIDocGeneratorMiddleware
    {
        private readonly RequestDelegate _next;
        private static Func<MethodInfo, Type, bool> _isAPIAction;
        private static Func<MethodInfo, Type, bool> _judgeAuthorized;
        private static List<object> _globalPossibleResponse;
        private static DocFormat _format;
        private static string _docAddress;

        public APIDocGeneratorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public static void ApplySettings(APIDocGeneratorSettings settings)
        {
            _isAPIAction = settings.IsAPIAction;
            _judgeAuthorized = settings.JudgeAuthorized;
            _globalPossibleResponse = settings.GlobalPossibleResponse;
            _format = settings.Format;
            _docAddress = settings.DocAddress.TrimStart('/').ToLower();
        }

        public async Task Invoke(HttpContext context)
        {
            if (_isAPIAction == null || _judgeAuthorized == null)
            {
                throw new ArgumentNullException();
            }
            if (context.Request.Path.ToString().Trim().Trim('/').ToLower() != _docAddress)
            {
                await _next.Invoke(context);
                return;
            }
            switch (_format)
            {
                case DocFormat.Json:
                    context.Response.ContentType = "application/json";
                    break;
                case DocFormat.Markdown:
                    context.Response.ContentType = "text/markdown";
                    break;
                default:
                    throw new InvalidDataException($"Invalid format: '{_format}'!");
            }
            context.Response.StatusCode = 200;
            var actionsMatches = new List<API>();
            var possibleControllers = Assembly
                .GetEntryAssembly()
                .GetTypes()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type))
                .ToList();
            foreach (var controller in possibleControllers ?? new List<Type>())
            {
                if (!IsController(controller))
                {
                    continue;
                }
                var controllerRoute = controller.GetCustomAttributes(typeof(RouteAttribute), true)
                            .Select(t => t as RouteAttribute)
                            .Select(t => t?.Template)
                            .FirstOrDefault();
                foreach (var method in controller.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                {
                    if (!IsAction(method) || !_isAPIAction(method, controller))
                    {
                        continue;
                    }
                    var args = GenerateArguments(method);
                    var possibleResponses = GetPossibleResponses(method);
                    var api = new API
                    {
                        ControllerName = controller.Name,
                        ActionName = method.Name,
                        IsPost = method.CustomAttributes.Any(t => t.AttributeType == typeof(HttpPostAttribute)),
                        Routes = method.GetCustomAttributes(typeof(RouteAttribute), true)
                            .Select(t => t as RouteAttribute)
                            .Select(t => t?.Template)
                            .Select(t => $"{controllerRoute}/{t}")
                            .ToList(),
                        Arguments = args,
                        AuthRequired = _judgeAuthorized(method, controller),
                        PossibleResponses = possibleResponses
                    };
                    if (!api.Routes.Any())
                    {
                        api.Routes.Add($"{api.ControllerName.TrimController()}/{api.ActionName}");
                    }
                    actionsMatches.Add(api);
                }
            }
            var generatedJsonDoc = JsonConvert.SerializeObject(actionsMatches);
            if (_format == DocFormat.Json)
            {
                await context.Response.WriteAsync(generatedJsonDoc);
            }
            else if (_format == DocFormat.Markdown)
            {
                var generator = new MarkDownDocGenerator();
                var groupedControllers = actionsMatches.GroupBy(t => t.ControllerName);
                string finalMarkDown = string.Empty;
                foreach (var controllerDoc in groupedControllers)
                {
                    finalMarkDown += generator.GenerateMarkDownForAPI(controllerDoc, $"{context.Request.Scheme}://{context.Request.Host}") + "\r\n--------\r\n";
                }
                await context.Response.WriteAsync(finalMarkDown);
            }
        }

        private string[] GetPossibleResponses(MethodInfo action)
        {
            var possibleList = action.GetCustomAttributes(typeof(APIProduces))
                .Select(t => (t as APIProduces)?.PossibleType)
                .Select(t => t.Make())
                .Select(JsonConvert.SerializeObject).ToList();
            possibleList.AddRange(
                _globalPossibleResponse.Select(JsonConvert.SerializeObject));
            return possibleList.ToArray();
        }

        private List<Argument> GenerateArguments(MethodInfo method)
        {
            var args = new List<Argument>();
            foreach (var param in method.GetParameters())
            {
                if (param.ParameterType.IsClass && param.ParameterType != typeof(string))
                {
                    foreach (var prop in param.ParameterType.GetProperties())
                    {
                        args.Add(new Argument
                        {
                            Name = GetArgumentName(prop, prop.Name),
                            Required = JudgeRequired(prop.PropertyType, prop.CustomAttributes),
                            Type = ConvertTypeToArgumentType(prop.PropertyType)
                        });
                    }
                }
                else
                {
                    args.Add(new Argument
                    {
                        Name = GetArgumentName(param, param.Name),
                        Required = !param.HasDefaultValue && JudgeRequired(param.ParameterType, param.CustomAttributes),
                        Type = ConvertTypeToArgumentType(param.ParameterType)
                    });
                }
            }
            return args;
        }

        private string GetArgumentName(ICustomAttributeProvider property, string defaultName)
        {
            var propName = defaultName;
            var fromQuery = property.GetCustomAttributes(typeof(IModelNameProvider), true).FirstOrDefault();
            if (fromQuery != null)
            {
                var queriedName = (fromQuery as IModelNameProvider)?.Name;
                if (!string.IsNullOrWhiteSpace(queriedName))
                {
                    propName = queriedName;
                }
            }
            return propName;
        }

        private bool IsController(Type type)
        {
            return
                type.Name.EndsWith("Controller") &&
                type.Name != "Controller" &&
                type.IsSubclassOf(typeof(ControllerBase)) &&
                type.IsPublic;
        }

        private bool IsAction(MethodInfo method)
        {
            return
                !method.IsAbstract &&
                !method.IsVirtual &&
                !method.IsStatic &&
                !method.IsConstructor &&
                !method.IsDefined(typeof(NonActionAttribute)) &&
                !method.IsDefined(typeof(ObsoleteAttribute));
        }

        private ArgumentType ConvertTypeToArgumentType(Type t)
        {
            return
                t == typeof(int) ? ArgumentType.Number :
                t == typeof(int?) ? ArgumentType.Number :
                t == typeof(long) ? ArgumentType.Number :
                t == typeof(long?) ? ArgumentType.Number :
                t == typeof(string) ? ArgumentType.Text :
                t == typeof(DateTime) ? ArgumentType.Datetime :
                t == typeof(DateTime?) ? ArgumentType.Datetime :
                t == typeof(bool) ? ArgumentType.Boolean :
                t == typeof(bool?) ? ArgumentType.Boolean :
                t == typeof(string[]) ? ArgumentType.Collection :
                t == typeof(List<string>) ? ArgumentType.Collection :
                ArgumentType.Unknown;
        }

        private bool JudgeRequired(Type source, IEnumerable<CustomAttributeData> attributes)
        {
            if (attributes.Any(t => t.AttributeType == typeof(RequiredAttribute)))
            {
                return true;
            }
            return
                source == typeof(int) || source == typeof(DateTime) || source == typeof(bool);
        }
    }

    public class API
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public bool AuthRequired { get; set; }
        public bool IsPost { get; set; }
        public List<Argument> Arguments { get; set; }
        public string[] PossibleResponses { get; set; }
        public List<string> Routes { get; set; }
    }

    public class Argument
    {
        public string Name { get; set; }
        public bool Required { get; set; }
        public ArgumentType Type { get; set; }
    }

    public enum ArgumentType
    {
        Text = 0,
        Number = 1,
        Boolean = 2,
        Datetime = 3,
        Collection = 4,
        Unknown = 5
    }
}
