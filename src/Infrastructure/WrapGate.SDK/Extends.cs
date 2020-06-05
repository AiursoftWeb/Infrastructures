using Aiursoft.Wrapgate.SDK.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.Wrapgate.SDK
{
    public static class Extends
    {
        public static IServiceCollection AddWrapgateServer(this IServiceCollection services, string wrapgateEndpoint)
        {
            return services.AddSingleton(new WrapgateLocator(wrapgateEndpoint));
        }
    }
}
