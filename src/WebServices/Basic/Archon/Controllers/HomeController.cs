using System;
using Aiursoft.Archon.SDK.Models;
using Aiursoft.Archon.Services;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.WebTools;
using Aiursoft.XelNaga.Tools;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Archon.Controllers
{
    [LimitPerMin]
    public class HomeController : ControllerBase
    {
        private readonly PrivateKeyStore _privateKeyStore;

        public HomeController(PrivateKeyStore privateKeyStore)
        {
            _privateKeyStore = privateKeyStore;
        }

        public IActionResult Index()
        {
            return this.Protocol(new IndexViewModel
            {
                Code = ErrorType.Success,
                Message = "Welcome to Archon server!",
                Exponent = _privateKeyStore.GetPrivateKey().Exponent?.BytesToBase64() ?? throw new NullReferenceException("Private key is null!"),
                Modulus = _privateKeyStore.GetPrivateKey().Modulus?.BytesToBase64() ?? throw new NullReferenceException("Private key is null!")
            });
        }
    }
}
