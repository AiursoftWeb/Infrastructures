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
        public AiurUrl(string Address)
        {
            this.Address = Address;
        }
        public AiurUrl(string Address, object Param) : this(Address)
        {
            var t = Param.GetType();
            foreach (var prop in t.GetProperties())
            {
                if (prop.GetValue(Param) != null)
                {
                    Params.Add(prop.Name, prop.GetValue(Param).ToString());
                }
            }
        }
        public AiurUrl(string Host, string Path, object Param) : this(Host + Path, Param) { }
        public AiurUrl(string Host, string ControllerName, string ActionName, object Param) : this(Host, $"/{WebUtility.UrlEncode(ControllerName)}/{WebUtility.UrlEncode(ActionName)}", Param) { }
        public override string ToString()
        {
            string Params = "?";
            foreach (var param in this.Params)
            {
                Params += param.Key + "=" + WebUtility.UrlEncode(param.Value) + "&";
            }
            return this.Address + Params.TrimEnd('?', '&');
        }
    }
}
