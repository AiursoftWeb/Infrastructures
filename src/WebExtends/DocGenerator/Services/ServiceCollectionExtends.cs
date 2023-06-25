using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Aiursoft.DocGenerator.Attributes;
using Aiursoft.DocGenerator.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;

namespace Aiursoft.DocGenerator.Services;

public enum DocFormat
{
    Json,
    Markdown
}

public class APIDocGeneratorSettings
{
    public string DocAddress = "doc";
    public DocFormat Format = DocFormat.Json;

    public Func<MethodInfo, Type, bool> IsAPIAction { get; set; } = (action, controller) =>
    {
        return
            action.CustomAttributes.Any(t => t.AttributeType == typeof(GenerateDoc)) ||
            controller.CustomAttributes.Any(t => t.AttributeType == typeof(GenerateDoc));
    };

    public Func<MethodInfo, Type, bool> RequiresAuthorized { get; set; } = (action, controller) =>
    {
        return
            action.CustomAttributes.Any(t => t.AttributeType == typeof(AuthorizeAttribute)) ||
            controller.CustomAttributes.Any(t => t.AttributeType == typeof(AuthorizeAttribute));
    };

    public List<object> GlobalApisPossibleResponses { get; set; } = new();
}

public static class ServiceCollectionExtends
{
    public static IApplicationBuilder UseAiursoftDocGenerator(
        this IApplicationBuilder app,
        Action<APIDocGeneratorSettings> options = null)
    {
        var defaultSettings = new APIDocGeneratorSettings();
        options?.Invoke(defaultSettings);
        APIDocGeneratorMiddleware.ApplySettings(defaultSettings);
        return app.UseMiddleware<APIDocGeneratorMiddleware>();
    }
}