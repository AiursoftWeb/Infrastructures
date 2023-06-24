using System.ComponentModel.DataAnnotations;
using Aiursoft.XelNaga.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Directory.SDK.Models.API.AppsAddressModels;

public class AppInfoAddressModel
{
    [Required]
    [IsGuidOrEmpty]
    [FromQuery(Name = "appid")]
    public string AppId { get; set; }
}