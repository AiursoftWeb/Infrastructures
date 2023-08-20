using System.Text;
using Microsoft.AspNetCore.Identity;
using Aiursoft.Directory.SDK.Models;
using Aiursoft.Directory.SDK.Services;
using Aiursoft.Directory.SDK.Models.ForApps.AddressModels;
using Aiursoft.Directory.SDK.Services.ToDirectoryServer;
using Microsoft.Extensions.Logging;

namespace Aiursoft.Identity.Services;

public class AuthService<T> where T : AiurUserBase, new()
{
    private readonly AccountService _accountService;
    private readonly DirectoryAppTokenService _directoryAppTokenService;
    private readonly SignInManager<T> _signInManager;
    private readonly ILogger<AuthService<T>> _logger;
    private readonly UserManager<T> _userManager;

    public AuthService(
        ILogger<AuthService<T>> logger,
        UserManager<T> userManager,
        SignInManager<T> signInManager,
        AccountService accountService,
        DirectoryAppTokenService directoryAppTokenService)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _accountService = accountService;
        _directoryAppTokenService = directoryAppTokenService;
    }

    public async Task<T> AuthApp(AuthResultAddressModel model, bool isPersistent = false)
    {
        var openId = await _accountService.CodeToOpenIdAsync(await _directoryAppTokenService.GetAccessTokenAsync(), model.Code);
        var userInfo = await _accountService.OpenIdToUserInfo(await _directoryAppTokenService.GetAccessTokenAsync(), openId.OpenId);
        var current = await _userManager.FindByIdAsync(userInfo.User.Id);
        if (current == null)
        {
            current = new T();
            current.Update(userInfo);
            var result = await _userManager.CreateAsync(current);
            if (!result.Succeeded)
            {
                var message = new StringBuilder();
                foreach (var error in result.Errors)
                {
                    _logger.LogError("Failed to create user based on Directory's user Info. Error: {@RegisterError}", error);
                    message.AppendLine(error.Description);
                }

                throw new InvalidOperationException(
                    $"The user info ({userInfo.User.Id}) we get could was not registered in our database because {message}.");
            }
        }
        else
        {
            current.Update(userInfo);
            await _userManager.UpdateAsync(current);
        }

        await _signInManager.SignInAsync(current, isPersistent);
        return current;
    }

    public async Task<T> Fetch(T user)
    {
        var userInfo = await _accountService.OpenIdToUserInfo(await _directoryAppTokenService.GetAccessTokenAsync(), user.Id);
        user.Update(userInfo);
        await _userManager.UpdateAsync(user);
        return user;
    }
}