using System.ComponentModel.DataAnnotations;
using Aiursoft.SDKTools.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Developer.SDK.Models.ApiAddressModels;

public class AppInfoAddressModel
{
    [Required]
    [IsGuidOrEmpty]
    [FromQuery(Name = "appid")]
    public string AppId { get; set; }
}