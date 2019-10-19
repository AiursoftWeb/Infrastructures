using Aiursoft.Gateway.Models.ThirdPartyAddressModels;
using Aiursoft.Pylon.Services.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Gateway.Controllers
{
    [Route("Third-party")]
    public class ThirdPartyController : Controller
    {
        private readonly IEnumerable<IAuthProvider> _authProviders;

        public ThirdPartyController(IEnumerable<IAuthProvider> authProviders)
        {
            _authProviders = authProviders;
        }

        [Route("Sign-in/{providerName}")]
        public IActionResult SignIn(SignInAddressModel model)
        {
            var provider = _authProviders.SingleOrDefault(t => t.GetName().ToLower() == model.ProviderName.ToLower());
            if(provider == null)
            {
                return NotFound();
            }
            var info = = provider.GetUserDetail(model.Code);
            return Json(info);
        }
    }
}
