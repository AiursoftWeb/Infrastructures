using Aiursoft.Scanner;
using Aiursoft.Warpgate.SDK.Models.ViewModels;
using Aiursoft.Warpgate.SDK.Services;
using Aiursoft.XelNaga.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;

namespace Aiursoft.Warpgate.SDK
{
    public static class Extends
    {
        public static IServiceCollection AddWarpgateServer(this IServiceCollection services, string warpgateEndpoint)
        {
            AsyncHelper.TryAsync(async () =>
            {
                var response = await SimpleHttp.DownloadAsString(warpgateEndpoint);
                var serverModel = JsonConvert.DeserializeObject<IndexViewModel>(response);
                services.AddSingleton(new WarpgateLocator(warpgateEndpoint, serverModel.WarpPattern));
            }, 5);
            services.AddLibraryDependencies();
            return services;
        }
    }
}
