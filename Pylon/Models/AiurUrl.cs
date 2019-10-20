using Aiursoft.Pylon.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Aiursoft.Pylon.Models
{
    public class AiurUrl
    {
        public string Address { get; set; }
        public Dictionary<string, string> Params { get; set; } = new Dictionary<string, string>();
        public AiurUrl(string address)
        {
            Address = address;
        }
        public AiurUrl(string address, object param) : this(address)
        {
            var t = param.GetType();
            foreach (var prop in t.GetProperties())
            {
                if (prop.GetValue(param) != null)
                {
                    var propName = prop.Name;
                    var fromQuery = prop.GetCustomAttributes(typeof(FromQueryAttribute), true).FirstOrDefault();
                    if (fromQuery != null)
                    {
                        propName = (fromQuery as FromQueryAttribute).Name;
                    }
                    Params.Add(propName, prop.GetValue(param).ToString());
                }
            }
        }
        public AiurUrl(string host, string path, object param) : this(host + path, param) { }
        public AiurUrl(string host, string controllerName, string actionName, object param) : this(host, $"/{WebUtility.UrlEncode(controllerName)}/{WebUtility.UrlEncode(actionName)}", param) { }
        public override string ToString()
        {
            string appendPart = "?";
            foreach (var param in this.Params)
            {
                appendPart += param.Key.ToLower() + "=" + param.Value.ToUrlEncoded() + "&";
            }
            return this.Address + appendPart.TrimEnd('?', '&');
        }
    }
}
