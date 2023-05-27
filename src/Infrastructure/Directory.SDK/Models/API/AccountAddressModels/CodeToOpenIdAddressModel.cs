using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Directory.SDK.Models.API.AccountAddressModels;

public class CodeToOpenIdAddressModel
{
    [Required] public virtual string AccessToken { get; set; }

    [Required] public virtual int Code { get; set; }
}