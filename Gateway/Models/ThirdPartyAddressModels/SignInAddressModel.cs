using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.API.OAuthAddressModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Aiursoft.Gateway.Models.ThirdPartyAddressModels
{
    public class SignInAddressModel
    {
        public string ProviderName { get; set; }
        public string Code { get; set; }
        public string State { get; set; }

        public FinishAuthInfo BuildOAuthInfo()
        {
            try
            {
                var values = State
                    .TrimStart('?')
                    .Split('&')
                    .Select(t => t.Split('='))
                    .Select(t => new KeyValuePair<string, string>(t[0].ToLower(), WebUtility.UrlDecode(t[1])));
                return new FinishAuthInfo
                {
                    AppId = values.SingleOrDefault(t => t.Key == "appid").Value,
                    RedirectUrl = values.SingleOrDefault(t => t.Key == "redirect-url").Value,
                    State = values.SingleOrDefault(t => t.Key == "state".ToLower()).Value,
                };
            }
            catch (Exception e) when (e is IndexOutOfRangeException || e is NullReferenceException)
            {
                throw new AiurAPIModelException(ErrorType.InvalidInput, "State is invalid!");
            }
        }
    }
}
