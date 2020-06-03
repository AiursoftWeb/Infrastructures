using Aiursoft.WrapGate.SDK.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.WrapGate.SDK
{
    public static class Extends
    {
        public static IServiceCollection AddWrapGateServer(this IServiceCollection services, string wrapgateEndpoint)
        {
            return services.AddSingleton(new WrapGateLocator(wrapgateEndpoint));
        }
    }
}
