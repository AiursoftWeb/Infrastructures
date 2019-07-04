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
        private static object _obj = new object();

        public LimitPerMin(int limit = 30)
        {
            _limit = limit;
        }

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
            if (DateTime.UtcNow - LastClearTime > TimeSpan.FromMinutes(1))
            {
                ClearMemory();
                LastClearTime = DateTime.UtcNow;
            }
            var tempDictionary = Copy();
            var path = context.HttpContext.Request.Path.ToString().ToLower();
            var ip = context.HttpContext.Connection.RemoteIpAddress.ToString();
            if (tempDictionary.ContainsKey(ip + path))
            {
                WriteMemory(ip + path, tempDictionary[ip + path] + 1);
                if (tempDictionary[ip + path] > _limit)
                {
                    context.HttpContext.Response.Headers.Add("retry-after", (60 - (int)(DateTime.UtcNow - LastClearTime).TotalSeconds).ToString());
                    context.Result = new StatusCodeResult((int)HttpStatusCode.TooManyRequests);
                }
            }
            else
            {
                tempDictionary[ip + path] = 1;
                WriteMemory(ip + path, 1);
            }
            context.HttpContext.Response.Headers.Add("x-rate-limit-limit", "1m");
            context.HttpContext.Response.Headers.Add("x-rate-limit-remaining", (_limit - tempDictionary[ip + path]).ToString());
        }
    }
}
