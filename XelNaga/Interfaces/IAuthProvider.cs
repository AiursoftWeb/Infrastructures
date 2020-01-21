using Aiursoft.Scanner.Interfaces;
using Aiursoft.XelNaga.Models;
using System.Threading.Tasks;

namespace Aiursoft.XelNaga.Services.Authentication
{
    public interface IAuthProvider : IScopedDependency
    {
        string GetName();
        string GetButtonColor();
        string GetButtonIcon();
        string GetSignInRedirectLink(AiurUrl state);
        string GetBindRedirectLink();
        Task<IUserDetail> GetUserDetail(string code, bool isBinding = false);
    }
}
