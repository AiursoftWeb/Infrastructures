using System;
using System.Collections.Generic;
using System.Net;
using Aiursoft.Handler.Models;
using Aiursoft.XelNaga.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Aiursoft.Handler.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class LimitPerMin : ActionFilterAttribute
{
    public static Dictionary<string, int> MemoryDictionary = new();
    public static DateTime LastClearTime = DateTime.UtcNow;
    private static readonly object _obj = new();

    private readonly int _limit;

    public LimitPerMin(int limit = 30)
    {
        _limit = limit;
    }

    public bool ReturnJson { get; set; } = true;

    public static void WriteMemory(string key, int value)
    {
        lock (_obj)
        {
            MemoryDictionary[key] = value;
        }
    }

    public static void ClearMemory()
    {
        lock (_obj)
        {
            MemoryDictionary.Clear();
        }
    }

    public static Dictionary<string, int> Copy()
    {
        lock (_obj)
        {
            return new Dictionary<string, int>(MemoryDictionary);
        }
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);
        if (context.HttpContext.Connection.RemoteIpAddress != null &&
            IPAddress.IsLoopback(context.HttpContext.Connection.RemoteIpAddress) && !EntryExtends.IsInUT())
            return;
        if (DateTime.UtcNow - LastClearTime > TimeSpan.FromMinutes(1))
        {
            ClearMemory();
            LastClearTime = DateTime.UtcNow;
        }

        var tempDictionary = Copy();
        var path = context.HttpContext.Request.Path.ToString().ToLower();
        var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        if (tempDictionary.ContainsKey(ip + path))
        {
            WriteMemory(ip + path, tempDictionary[ip + path] + 1);
            if (tempDictionary[ip + path] > _limit)
            {
                context.HttpContext.Response.Headers.Add("retry-after",
                    (60 - (int)(DateTime.UtcNow - LastClearTime).TotalSeconds).ToString());
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                if (ReturnJson)
                    context.Result = new JsonResult(new AiurProtocol
                    {
                        Code = ErrorType.TooManyRequests,
                        Message = "You are requesting our API too frequently and your IP is blocked."
                    });
                else
                    context.Result = new StatusCodeResult((int)HttpStatusCode.TooManyRequests);
            }
        }
        else
        {
            tempDictionary[ip + path] = 1;
            WriteMemory(ip + path, 1);
        }

        context.HttpContext.Response.Headers.Add("x-rate-limit-limit", "1m");
        context.HttpContext.Response.Headers.Add("x-rate-limit-remaining",
            (_limit - tempDictionary[ip + path]).ToString());
    }
}