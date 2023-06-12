using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Aiursoft.Directory.SDK.Models;

public class DirectoryApp
{
    [Obsolete(error: true, message: "This method is only for framework!")]
    public DirectoryApp()
    {
    }

    public DirectoryApp(string name, string description, string iconPath)
    {
        AppId = Guid.NewGuid().ToString("N");
        AppSecret = Guid.NewGuid().ToString("N");
        AppName = name;
        AppDescription = description;
        IconPath = iconPath;
    }

    [Key] public string AppId { get; set; }

    public string AppSecret { get; set; }

    public string AppName { get; set; }

    /// <summary>
    /// sitename/filepath/filename.extension
    /// </summary>
    public string IconPath { get; set; }

    public string AppDescription { get; set; }

    [DisplayFormat(DataFormatString = "{0:yyyy - MM - dd}")]
    public DateTime AppCreateTime { get; set; } = DateTime.UtcNow;

    public bool EnableOAuth { get; set; } = true;
    public bool ForceInputPassword { get; set; }
    public bool ForceConfirmation { get; set; } = true;
    public bool DebugMode { get; set; }
    public string AppDomain { get; set; }
    public string AppFailCallbackUrl { get; set; }
    public bool ViewOpenId { get; set; } = true;
    public bool ViewPhoneNumber { get; set; }
    public bool ChangePhoneNumber { get; set; }
    public bool ConfirmEmail { get; set; }
    public bool ChangeBasicInfo { get; set; }
    public bool ChangePassword { get; set; }
    public bool ChangeGrantInfo { get; set; }
    public bool ViewAuditLog { get; set; }
    public bool ManageSocialAccount { get; set; }
    public bool TrustedApp { get; set; }

    [Url]
    [Display(Name = "Privacy Statement Url")]
    public string PrivacyStatementUrl { get; set; }

    [Url]
    [Display(Name = "License Url")] 
    public string LicenseUrl { get; set; }
}