using Aiursoft.Scanner.Interfaces;
using Aiursoft.WWW.Services.BingModels;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aiursoft.WWW.Services
{
    public class BingTranslator : IScopedDependency
    {
        public static string APIKey;

        private async Task<string> CallTranslateAPI(string inputJson, string targetLanguage)
        {
            var apiAddress = $"https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&to={targetLanguage}";
            var client = new RestClient(apiAddress);
            var request = new RestRequest();
            request
                .AddHeader("Ocp-Apim-Subscription-Key", APIKey)
                .AddHeader("Content-Type", "application/json")
                .AddParameter("undefined", inputJson, ParameterType.RequestBody);

#warning Replace RestSharp with pure HTTP Client.
            var json = (await client.PostAsync(request)).Content;
            return json;
        }

        public async Task<string> CallTranslate(string input, string targetLanguage)
        {
            var inputSource = new List<Translation>
            {
                new Translation { Text = input }
            };
            var bingResponse = await CallTranslateAPI(JsonConvert.SerializeObject(inputSource), targetLanguage);
            var result = JsonConvert.DeserializeObject<List<BingResponse>>(bingResponse);
            return result[0].Translations[0].Text;
        }
    }
}
