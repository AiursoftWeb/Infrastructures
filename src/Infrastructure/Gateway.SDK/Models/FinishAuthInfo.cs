using System.ComponentModel.DataAnnotations;
using Aiursoft.SDKTools.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Gateway.SDK.Models;

public class FinishAuthInfo
{
    [Required]
    [IsGuidOrEmpty]
    [FromQuery(Name = "appid")]
    public string AppId { get; set; }

    [Url]
    [Required]
    [FromQuery(Name = "redirect_uri")]
    public string RedirectUri { get; set; }

    [FromQuery(Name = "state")] public string State { get; set; }
}