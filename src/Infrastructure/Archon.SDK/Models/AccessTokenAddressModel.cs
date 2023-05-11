using System.ComponentModel.DataAnnotations;
using Aiursoft.SDKTools.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Archon.SDK.Models;

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