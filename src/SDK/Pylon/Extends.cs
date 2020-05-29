using Aiursoft.Archon.SDK;
using Aiursoft.Gateway.SDK;
using Aiursoft.Gateway.SDK.Models;
using Aiursoft.Gateway.SDK.Models.API.OAuthAddressModels;
using Aiursoft.Observer.SDK;
using Aiursoft.Probe.SDK;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.Authentication;
using Aiursoft.SDK;
using Aiursoft.XelNaga.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace Aiursoft.Pylon
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
            string archonEndpoint) where TUser : AiurUserBase, new()
        {
            if (Assembly.GetEntryAssembly().FullName.StartsWith("ef"))
            {
                Console.WriteLine("Calling from Entity Framework! Skipped dependencies management!");
                return services;
            }
            services.AddObserverServer(); // For error reporting.
            services.AddArchonServer(archonEndpoint); // For token exchanging.
            services.AddProbeServer(); // For file storaging.
            services.AddGatewayServer(); // For authentication.
            services.AddBasic(abstracts: typeof(IAuthProvider));
            services.AddScoped<UserImageGenerator<TUser>>();
            services.AddScoped<AuthService<TUser>>();
            return services;
        }
    }
}
