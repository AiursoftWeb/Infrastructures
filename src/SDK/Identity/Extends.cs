using Aiursoft.Archon.SDK;
using Aiursoft.Gateway.SDK;
using Aiursoft.Gateway.SDK.Models;
using Aiursoft.Gateway.SDK.Models.API.OAuthAddressModels;
using Aiursoft.Identity.Services;
using Aiursoft.Identity.Services.Authentication;
using Aiursoft.Observer.SDK;
using Aiursoft.Probe.SDK;
using Aiursoft.SDK;
using Aiursoft.XelNaga.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Security.Claims;

namespace Aiursoft.Identity
{
    public static class Extends
    {
        public static IActionResult SignOutRootServer(this Controller controller, string apiServerAddress, AiurUrl viewingUrl)
        {
            var request = controller.HttpContext.Request;
            var serverPosition = $"{request.Scheme}://{request.Host}{viewingUrl}";
            var toRedirect = new AiurUrl(apiServerAddress, "OAuth", "UserSignOut", new UserSignOutAddressModel
            {
                ToRedirect = serverPosition
            });
            return controller.Redirect(toRedirect.ToString());
        }

        public static string GetUserId(this ClaimsPrincipal user) =>
            user.FindFirstValue(ClaimTypes.NameIdentifier);

        public static IServiceCollection AddAiursoftIdentity<TUser>(this IServiceCollection services,
            string archonEndpoint,
            string observerEndpoint,
            string probeEndpoint,
            string gateEndpoint) where TUser : AiurUserBase, new()
        {
            services.AddObserverServer(observerEndpoint); // For error reporting.
            services.AddArchonServer(archonEndpoint); // For token exchanging.
            services.AddProbeServer(probeEndpoint); // For file storaging.
            services.AddGatewayServer(gateEndpoint); // For authentication.
            services.AddAiursoftSDK(Assembly.GetCallingAssembly(), abstracts: typeof(IAuthProvider));
            services.AddScoped<UserImageGenerator<TUser>>();
            services.AddScoped<AuthService<TUser>>();
            return services;
        }
    }
}
