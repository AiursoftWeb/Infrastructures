using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Probe.SDK.Models.FoldersAddressModels;

public class DeleteFolderAddressModel
{
    [FromRoute] [Required] public string SiteName { get; set; }

    [FromRoute] public string FolderNames { get; set; }
}

public class DeleteFolderFormModel
{
    [Required] public string AccessToken { get; set; }
}