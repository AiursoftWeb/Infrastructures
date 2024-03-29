using System.Reflection;
using System.Security.Claims;
using Aiursoft.AiurProtocol;
using Aiursoft.Directory.SDK.Models;
using Aiursoft.Directory.SDK.Models.API.OAuthAddressModels;
using Aiursoft.Identity.Services;
using Aiursoft.Identity.Services.Authentication;
using Aiursoft.Observer.SDK;
using Aiursoft.Probe.SDK;
using Aiursoft.Directory.SDK;
using Aiursoft.SDK;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.Identity;

public static class Extends
{
    public static IActionResult SignOutRootServer(
        this Controller controller, 
        string directoryEndpoint,
        string viewingPath)
    {
        var request = controller.HttpContext.Request;
        var serverPosition = $"{request.Scheme}://{request.Host}/{viewingPath.TrimStart('/')}";
        var toRedirect = new AiurApiEndpoint(directoryEndpoint, "OAuth", "UserSignOut", new UserSignOutAddressModel
        {
            ToRedirect = serverPosition
        });
        return controller.Redirect(toRedirect.ToString());
    }

    public static string GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    public static IServiceCollection AddAiursoftIdentity<TUser>(
        this IServiceCollection services,
        IConfigurationSection observerConfig,
        IConfigurationSection probeConfig,
        IConfigurationSection authenticationConfig) where TUser : AiurUserBase, new()
    {
        services.AddAiursoftProbe(probeConfig); // For file storage like user's avatar.
        services.AddAiursoftObserver(observerConfig); // For error reporting.
        services.AddAiursoftAppAuthentication(authenticationConfig); // For authentication.

        services.AddScannedServices(Assembly.GetCallingAssembly(), typeof(IAuthProvider));
        services.AddScoped<UserImageGenerator<TUser>>();
        services.AddScoped<AuthService<TUser>>();
        return services;
    }
}