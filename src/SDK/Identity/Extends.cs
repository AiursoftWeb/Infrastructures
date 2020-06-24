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
using System;
using System.Reflection;

namespace Aiursoft.Identity
{
    public static class Extends
    {
        public static IActionResult SignOutRootServer(this Controller controller, string apiServerAddress, AiurUrl viewingUrl)
        {
            var request = controller.HttpContext.Request;
            string serverPosition = $"{request.Scheme}://{request.Host}{viewingUrl}";
            var toRedirect = new AiurUrl(apiServerAddress, "OAuth", "UserSignout", new UserSignoutAddressModel
            {
                ToRedirect = serverPosition
            });
            return controller.Redirect(toRedirect.ToString());
        }

        public static IServiceCollection AddAiurDependenciesWithIdentity<TUser>(this IServiceCollection services,
            string archonEndpoint,
            string observerEndpoint,
            string probeEndpoint,
            string gateEndpoint) where TUser : AiurUserBase, new()
        {
            var entry = Assembly.GetEntryAssembly();
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }
            if (entry.FullName?.StartsWith("ef") ?? false)
            {
                Console.WriteLine("Calling from Entity Framework! Skipped dependencies management!");
                return services;
            }
            services.AddObserverServer(observerEndpoint); // For error reporting.
            services.AddArchonServer(archonEndpoint); // For token exchanging.
            services.AddProbeServer(probeEndpoint); // For file storaging.
            services.AddGatewayServer(gateEndpoint); // For authentication.
            services.AddBasic(abstracts: typeof(IAuthProvider));
            services.AddScoped<UserImageGenerator<TUser>>();
            services.AddScoped<AuthService<TUser>>();
            return services;
        }
    }
}
