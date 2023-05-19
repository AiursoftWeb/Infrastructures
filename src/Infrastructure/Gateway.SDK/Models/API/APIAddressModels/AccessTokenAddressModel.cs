using System.ComponentModel.DataAnnotations;
using Aiursoft.SDKTools.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Gateway.SDK.Models.API.APIAddressModels;

public class AccessTokenAddressModel
{
    [Required]
    [IsGuidOrEmpty]
    [FromQuery(Name = "appId")]
    public virtual string AppId { get; set; }

    [Required]
    [IsGuidOrEmpty]
    [FromQuery(Name = "appSecret")]
    public virtual string AppSecret { get; set; }
}