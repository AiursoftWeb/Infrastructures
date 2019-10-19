using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services.Authentication
{
    public interface IAuthProvider
    {
        string GetName();
        string GetButtonColor();
        string GetButtonIcon();
        Task<IUserDetail> GetUserDetail(string code);
    }
}
