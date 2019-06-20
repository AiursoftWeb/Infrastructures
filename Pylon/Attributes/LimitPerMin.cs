using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Aiursoft.Pylon.Attributes
{
    public class LimitPerMin : ActionFilterAttribute
    {
        public static Dictionary<string, int> MemoryDictionary = new Dictionary<string, int>();
        public static DateTime LastClearTime = DateTime.UtcNow;

        private readonly int _limit;

        public LimitPerMin(int limit = 60)
        {
            _limit = limit;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (DateTime.UtcNow - LastClearTime > TimeSpan.FromMinutes(1))
            {
                MemoryDictionary = new Dictionary<string, int>();
                LastClearTime = DateTime.UtcNow;
            }
            var path = context.HttpContext.Request.Path.ToString().ToLower();
            var ip = context.HttpContext.Connection.RemoteIpAddress.ToString();
            if (MemoryDictionary.ContainsKey(ip + path))
            {
                MemoryDictionary[ip + path]++;
                if (MemoryDictionary[ip + path] > _limit)
                {
                    context.HttpContext.Response.Headers.Add("retry-after", (60 - (int)(DateTime.UtcNow - LastClearTime).TotalSeconds).ToString());
                    context.Result = new StatusCodeResult((int)HttpStatusCode.TooManyRequests);
                }
            }
            else
            {
                MemoryDictionary[ip + path] = 1;
            }
            context.HttpContext.Response.Headers.Add("x-rate-limit-limit", "1m");
            context.HttpContext.Response.Headers.Add("x-rate-limit-remaining", (_limit - MemoryDictionary[ip + path]).ToString());
        }
    }
}
