using Aiursoft.Gateway.SDK.Models;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Aiursoft.Gateway.Models.ThirdPartyAddressModels
{
    public class SignInAddressModel
    {
        [FromRoute]
        public string ProviderName { get; set; }
        [FromQuery(Name = "code")]
        public string Code { get; set; }
        [FromQuery(Name = "state")]
        public string State { get; set; }

        public FinishAuthInfo BuildOAuthInfo()
        {
            try
            {
                var values = State
                    .TrimStart('?')
                    .Split('&')
                    .Select(t => t.Split('='))
                    .Select(t => new KeyValuePair<string, string>(t[0].ToLower(), WebUtility.UrlDecode(t[1])))
                    .ToArray();
                return new FinishAuthInfo
                {
                    AppId = values.SingleOrDefault(t => t.Key == "appid").Value,
                    RedirectUri = values.SingleOrDefault(t => t.Key == "redirect_uri").Value,
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
