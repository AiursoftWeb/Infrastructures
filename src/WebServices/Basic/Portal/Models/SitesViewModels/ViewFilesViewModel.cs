using System;
using System.Collections.Generic;
using Aiursoft.Directory.SDK.Models;
using Aiursoft.Portal.Models.AppsViewModels;
using Aiursoft.Probe.SDK.Models;

namespace Aiursoft.Portal.Models.SitesViewModels;

public class ViewFilesViewModel : AppLayoutModel
{
    [Obsolete("This method is only for framework", true)]
    public ViewFilesViewModel()
    {
    }

    public ViewFilesViewModel(PortalUser user, IReadOnlyCollection<DirectoryApp> hisApps) : base(user, hisApps)
    {
    }

    public Folder Folder { get; set; }
    public string AppId { get; set; }
    public string SiteName { get; set; }
    public string Path { get; set; }
    public string AppName { get; set; }
}