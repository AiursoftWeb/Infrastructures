using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Probe.SDK.Models.FilesAddressModels;

public class RenameFileAddressModel
{
    [Required] [FromRoute] public string SiteName { get; set; }

    [Required] [FromRoute] public string FolderNames { get; set; }

}

public class RenameFileFormModel
{
    [Required] public string TargetFileName { get; set; }
    [Required] public string AccessToken { get; set; }
}