using Aiursoft.Pylon.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services.Authentication
{
    public interface IAuthProvider : IScopedDependency
    {
        string GetName();
        string GetButtonColor();
        string GetButtonIcon();
        string GetSignInRedirectLink(string state);
        Task<IUserDetail> GetUserDetail(string code);
    }
}
