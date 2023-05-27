using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Directory.Models.ThirdPartyAddressModels;

public class BindAccountAddressModel
{
    [FromRoute] public string ProviderName { get; set; }

    [FromQuery(Name = "code")] public string Code { get; set; }
}