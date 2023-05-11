using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using Aiursoft.XelNaga.Tools;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Aiursoft.XelNaga.Models;

public class AiurUrl
{
    public AiurUrl(string address)
    {
        Address = address;
    }

    public AiurUrl(string address, object param) : this(address)
    {
        var t = param.GetType();
        foreach (var prop in t.GetProperties())
        {
            if (prop.GetValue(param) == null) continue;

            var propName = prop.Name;
            var propValue = prop.GetValue(param)?.ToString();
            var fromQuery = prop.GetCustomAttributes(typeof(IModelNameProvider), true).FirstOrDefault();
            if (fromQuery is IModelNameProvider nameProvider && nameProvider.Name != null) propName = nameProvider.Name;
            if (prop.PropertyType == typeof(DateTime))
                if (prop.GetValue(param) is DateTime time)
                    propValue = time.ToString("o", CultureInfo.InvariantCulture);
            if (!string.IsNullOrWhiteSpace(propValue)) Params.Add(propName, propValue);
        }
    }

    public AiurUrl(string host, string path, object param) : this(host + path, param)
    {
    }

    public AiurUrl(string host, string controllerName, string actionName, object param) : this(host,
        $"/{WebUtility.UrlEncode(controllerName)}/{WebUtility.UrlEncode(actionName)}", param)
    {
    }

    public string Address { get; set; }
    public Dictionary<string, string> Params { get; } = new();

    public override string ToString()
    {
        var appendPart = Params.Aggregate("?", (c, p) =>
            $"{c}{p.Key.ToLower()}={p.Value.ToUrlEncoded()}&");
        return Address + appendPart.TrimEnd('?', '&');
    }

    public bool IsLocalhost()
    {
        return Address.StartsWith("http://localhost") ||
               Address.StartsWith("https://localhost") ||
               Address.StartsWith("http://127.0.0.1") ||
               Address.StartsWith("https://127.0.0.1") ||
               Address.StartsWith("http://::1") ||
               Address.StartsWith("https://::1");
    }
}