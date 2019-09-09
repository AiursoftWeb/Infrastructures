using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Middlewares
{
    public class APIDocGeneratorMiddleware
    {
        private IConfiguration _configuration { get; }
        private RequestDelegate _next;

        public APIDocGeneratorMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _configuration = configuration;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.ToString().ToLower() != "/doc")
            {
                await _next.Invoke(context);
                return;
            }
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 200;
            var actionsMatches = new List<API>();
            foreach (var controller in Assembly.GetEntryAssembly().GetTypes().Where(type => typeof(Controller).IsAssignableFrom(type)))
            {
                if (!IsController(controller))
                {
                    continue;
                }
                foreach (var method in controller.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                {
                    if (!IsAction(method) || !IsAPIAction(method, controller))
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
                        Arguments = args,
                        AuthRequired = JudgeAuthorized(method, controller),
                        RequiresFile = JudgeRequiredFile(method, controller),
                        PossibleResponses = possibleResponses
                    };
                    actionsMatches.Add(api);
                }
            }
            await context.Response.WriteAsync(JsonConvert.SerializeObject(actionsMatches));
            return;
        }

        private string[] GetPossibleResponses(MethodInfo action)
        {
            try
            {
                var possibleList = action.GetCustomAttributes(typeof(APIProduces))
                    .Select(t => (t as APIProduces).PossibleType)
                    .Select(t => InstanceMaker.Make(t))
                    .Select(t => JsonConvert.SerializeObject(t)).ToList();
                possibleList.Add(JsonConvert.SerializeObject(new AiurProtocol
                {
                    Code = ErrorType.WrongKey,
                    Message = "Some error."
                }));
                possibleList.Add(JsonConvert.SerializeObject(new AiurCollection<string>(new List<string> { "Some item is invalid!" })
                {
                    Code = ErrorType.InvalidInput,
                    Message = "Your input contains several errors!"
                }));
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
                type.Name != "HomeController" &&
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

        private bool IsAPIAction(MethodInfo action, Type controller)
        {
            return
                action.CustomAttributes.Any(t => t.AttributeType == typeof(APIExpHandler)) ||
                controller.CustomAttributes.Any(t => t.AttributeType == typeof(APIExpHandler)) ||
                action.CustomAttributes.Any(t => t.AttributeType == typeof(APIModelStateChecker)) ||
                controller.CustomAttributes.Any(t => t.AttributeType == typeof(APIModelStateChecker));
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

        private bool JudgeAuthorized(MethodInfo action, Type controller)
        {
            return
                action.CustomAttributes.Any(t => t.AttributeType == typeof(AiurForceAuth)) ||
                controller.CustomAttributes.Any(t => t.AttributeType == typeof(AiurForceAuth));
        }

        private bool JudgeRequiredFile(MethodInfo action, Type controller)
        {
            return
                action.CustomAttributes.Any(t => t.AttributeType == typeof(FileChecker)) ||
                controller.CustomAttributes.Any(t => t.AttributeType == typeof(FileChecker));
        }
    }

    public class API
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public bool AuthRequired { get; set; }
        public bool IsPost { get; set; }
        public List<Argument> Arguments { get; set; }
        public bool RequiresFile { get; set; }
        public string[] PossibleResponses { get; set; }
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
