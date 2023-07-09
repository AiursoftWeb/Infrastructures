using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Probe.SDK.Models.FilesAddressModels;

public class CopyFileAddressModel
{
    [Required] [FromRoute] public string SiteName { get; set; }

    [Required] [FromRoute] public string FolderNames { get; set; }
}

public class CopyFileFormModel
{
    // TODO: Unify directory token.

    [Required] public string AccessToken { get; set; }
    
    [Required] public string TargetSiteName { get; set; }

    public string TargetFolderNames { get; set; }
}