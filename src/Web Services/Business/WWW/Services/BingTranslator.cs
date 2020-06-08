using Aiursoft.WWW.Services.BingModels;
using Aiursoft.WWW.Services;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;

namespace Aiursoft.WWW.Services
{
    public class BingTranslator
    {
        public static string APIKey;

        private string CallTranslateAPI(string inputJson, string targetLanguage)
        {
            var apiAddress = $"https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&to={targetLanguage}";
            var client = new RestClient(apiAddress);
            var request = new RestRequest(Method.POST);
            request
                .AddHeader("Ocp-Apim-Subscription-Key", APIKey)
                .AddHeader("Content-Type", "application/json")
                .AddParameter("undefined", inputJson, ParameterType.RequestBody);

            var json = client.Execute(request).Content;
            return json;
        }

        public string CallTranslate(string input, string targetLanguage)
        {
            var inputSource = new List<Translation>
            {
                new Translation { Text = input }
            };
            var bingResponse = CallTranslateAPI(JsonConvert.SerializeObject(inputSource), targetLanguage);
            var result = JsonConvert.DeserializeObject<List<BingResponse>>(bingResponse);
            return result[0].Translations[0].Text;
        }
    }
}
