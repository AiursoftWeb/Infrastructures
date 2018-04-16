using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.OSS.SecretAddressModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services.ToOSSServer
{
    public static class SecretService
    {
        public static async Task<AiurValue<string>> GenerateAsync(int id, string accessToken)
        {
            var httpContainer = new HTTPService();
            var url = new AiurUrl(Values.OssServerAddress, "Secret", "Generate", new GenerateAddressModel
            {
                Id = id,
                AccessToken = accessToken
            });
            var result = await httpContainer.Get(url);
            var jResult = JsonConvert.DeserializeObject<AiurValue<string>>(result);
            if (jResult.code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }
    }
}
