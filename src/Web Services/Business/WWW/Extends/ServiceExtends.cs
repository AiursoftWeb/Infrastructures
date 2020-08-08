using Aiursoft.WWW.Services;
using Aiursoft.XelNaga.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Aiursoft.WWW.Extends
{
    public static class ServiceExtends
    {
        public static IServiceCollection UseBlacklistFromAddress(this IServiceCollection services, string address)
        {
            AsyncHelper.TryAsyncThreeTimes(async () =>
            {
                var list = await new WebClient().DownloadStringTaskAsync(address);
                var provider = new BlackListPorivder(list.Split('\n'));
                services.AddSingleton(provider);

            });
            return services;
        }
    }
}
