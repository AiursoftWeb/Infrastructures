using System.Collections.Generic;
using System.Threading.Tasks;
using Aiursoft.Scanner.Abstract;
using Aiursoft.WWW.Services.BingModels;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;

namespace Aiursoft.WWW.Services;

public class BingTranslator : IScopedDependency
{
    private readonly IConfiguration _configuration;

    public BingTranslator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private async Task<string> CallTranslateAPI(string inputJson, string targetLanguage)
    {
        var apiAddress = $"https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&to={targetLanguage}";
        var client = new RestClient(apiAddress);
        var request = new RestRequest();
        request
            .AddHeader("Ocp-Apim-Subscription-Key", _configuration["TranslateAPIKey"])
            .AddHeader("Content-Type", "application/json")
            .AddParameter("undefined", inputJson, ParameterType.RequestBody);

        var json = (await client.PostAsync(request)).Content;
        return json;
    }

    public async Task<string> CallTranslate(string input, string targetLanguage)
    {
        var inputSource = new List<Translation>
        {
            new() { Text = input }
        };
        var bingResponse = await CallTranslateAPI(JsonConvert.SerializeObject(inputSource), targetLanguage);
        var result = JsonConvert.DeserializeObject<List<BingResponse>>(bingResponse);
        return result[0].Translations[0].Text;
    }
}