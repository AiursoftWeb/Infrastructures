using System.ComponentModel.DataAnnotations;
using Aiursoft.XelNaga.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Directory.SDK.Models.API.AppsAddressModels;

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