using System.ComponentModel.DataAnnotations;
using Aiursoft.CSTools.Attributes;

namespace Aiursoft.Probe.SDK.Models.FoldersAddressModels;

public class CreateNewFolderAddressModel
{
    [Required] public string SiteName { get; set; }

    public string FolderNames { get; set; }
}

public class CreateNewFolderFormModel
{
    [Required] public string AccessToken { get; set; }
    
    [Required] [ValidFolderName] public string NewFolderName { get; set; }

    public bool RecursiveCreate { get; set; }
}