using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Probe.SDK.Models.FilesAddressModels;

public class DeleteFileAddressModel
{
    [Required] [FromRoute] public string SiteName { get; set; }

    [Required] [FromRoute] public string FolderNames { get; set; }
}

public class DeleteFileFormModel
{
    [Required] public string AccessToken { get; set; }
}