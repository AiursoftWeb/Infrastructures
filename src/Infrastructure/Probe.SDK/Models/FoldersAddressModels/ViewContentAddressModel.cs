using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Probe.SDK.Models.FoldersAddressModels;

public class ViewContentAddressModel
{
    [Required] public string AccessToken { get; set; }

    [Required] [FromRoute] public string SiteName { get; set; }

    [FromRoute] public string FolderNames { get; set; }
}