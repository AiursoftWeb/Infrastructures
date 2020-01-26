using Aiursoft.DocGenerator.Attributes;
using Aiursoft.DocGenerator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        }

        public async Task Invoke(HttpContext context)
        {
            if (_isAPIAction == null || _judgeAuthorized == null)
            {
                throw new ArgumentNullException();
            }
            if (context.Request.Path.ToString().Trim().Trim('/').ToLower() != "doc")
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
                    throw new NotImplementedException($"Invalid format: '{_format}'!");
            }
            context.Response.StatusCode = 200;
            var actionsMatches = new List<API>();
            var possibleControllers = Assembly.GetEntryAssembly().GetTypes().Where(type => typeof(Controller).IsAssignableFrom(type));
            foreach (var controller in possibleControllers)
            {
                if (!IsController(controller))
                {
                    continue;
                }
                var controllerRoute = controller.GetCustomAttributes(typeof(RouteAttribute), true)
                            .Select(t => t as RouteAttribute)
                            .Select(t => t.Template)
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
                            .Select(t => t.Template)
                            .Select(t => $"{controllerRoute}/{t}")
                            .ToArray(),
                        Arguments = args,
                        AuthRequired = _judgeAuthorized(method, controller),
                        PossibleResponses = possibleResponses
                    };
                    actionsMatches.Add(api);
                }
            }
            var generatoredJsonDoc = JsonConvert.SerializeObject(actionsMatches);
            if (_format == DocFormat.Json)
            {
                await context.Response.WriteAsync(generatoredJsonDoc);
            }
            else if (_format == DocFormat.Markdown)
            {
                var generator = new MarkDownDocGenerator();
                var grouppedControllers = actionsMatches.GroupBy(t => t.ControllerName);
                string finalMarkDown = string.Empty;
                foreach (var controllerDoc in grouppedControllers)
                {
                    finalMarkDown += generator.GenerateMarkDownForAPI(controllerDoc, $"{context.Request.Scheme}://{context.Request.Host}") + "\r\n--------\r\n";
                }
                await context.Response.WriteAsync(finalMarkDown);
            }
            return;
        }

        private string[] GetPossibleResponses(MethodInfo action)
        {
            try
            {
                var possibleList = action.GetCustomAttributes(typeof(APIProduces))
                    .Select(t => (t as APIProduces).PossibleType)
                    .Select(t => t.Make())
                    .Select(t => JsonConvert.SerializeObject(t)).ToList();
                possibleList.AddRange(
                    _globalPossibleResponse.Select(t => JsonConvert.SerializeObject(t)));
                return possibleList.ToArray();
            }
            catch (Exception e)
            {
                throw e;
            }
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
                            Name = prop.Name,
                            Required = JudgeRequired(prop.PropertyType, prop.CustomAttributes),
                            Type = ConvertTypeToArgumentType(prop.PropertyType)
                        });
                    }
                }
                else
                {
                    args.Add(new Argument
                    {
                        Name = param.Name,
                        Required = param.HasDefaultValue ? false : JudgeRequired(param.ParameterType, param.CustomAttributes),
                        Type = ConvertTypeToArgumentType(param.ParameterType)
                    });
                }
            }
            return args;
        }

        private bool IsController(Type type)
        {
            return
                type.Name.EndsWith("Controller") &&
                type.Name != "Controller" &&
                type.IsSubclassOf(typeof(Controller)) &&
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
                t == typeof(int) ? ArgumentType.number :
                t == typeof(int?) ? ArgumentType.number :
                t == typeof(long) ? ArgumentType.number :
                t == typeof(long?) ? ArgumentType.number :
                t == typeof(string) ? ArgumentType.text :
                t == typeof(DateTime) ? ArgumentType.datetime :
                t == typeof(DateTime?) ? ArgumentType.datetime :
                t == typeof(bool) ? ArgumentType.boolean :
                t == typeof(bool?) ? ArgumentType.boolean :
                t == typeof(string[]) ? ArgumentType.collection :
                t == typeof(List<string>) ? ArgumentType.collection :
                ArgumentType.unknown;
        }

        private bool JudgeRequired(Type source, IEnumerable<CustomAttributeData> attributes)
        {
            if (attributes.Any(t => t.AttributeType == typeof(RequiredAttribute)))
            {
                return true;
            }
            return
                source == typeof(int) ? true :
                source == typeof(DateTime) ? true :
                source == typeof(bool) ? true :
                false;
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
        public string[] Routes { get; set; }
    }

    public class Argument
    {
        public string Name { get; set; }
        public bool Required { get; set; }
        public ArgumentType Type { get; set; }
    }

    public enum ArgumentType
    {
        text = 0,
        number = 1,
        boolean = 2,
        datetime = 3,
        collection = 4,
        unknown = 5
    }
}
