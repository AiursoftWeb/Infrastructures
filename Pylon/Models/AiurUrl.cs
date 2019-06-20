using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

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
                    Params.Add(prop.Name, prop.GetValue(param).ToString());
                }
            }
        }
        public AiurUrl(string host, string path, object param) : this(host + path, param) { }
        public AiurUrl(string host, string controllerName, string actionName, object param) : this(host, $"/{WebUtility.UrlEncode(controllerName)}/{WebUtility.UrlEncode(actionName)}", param) { }
        public override string ToString()
        {
            string Params = "?";
            foreach (var param in this.Params)
            {
                Params += param.Key.ToLower() + "=" + WebUtility.UrlEncode(param.Value) + "&";
            }
            return this.Address + Params.TrimEnd('?', '&');
        }
    }
}
