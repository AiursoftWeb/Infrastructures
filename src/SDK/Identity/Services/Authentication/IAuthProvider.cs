using System.Threading.Tasks;
using Aiursoft.Scanner.Abstract;
using Aiursoft.XelNaga.Models;

namespace Aiursoft.Identity.Services.Authentication;

public interface IAuthProvider : IScopedDependency
{
    bool IsEnabled();
    string GetName();
    string GetSettingsPage();
    string GetButtonColor();
    string GetButtonIcon();
    string GetSignInRedirectLink(string state);
    string GetBindRedirectLink();
    Task<IUserDetail> GetUserDetail(string code, bool isBinding = false);
}