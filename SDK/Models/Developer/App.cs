using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.SDK.Models.Developer
{
    public class App
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public App() { }
        public App(string name, string description, Category category, Platform platform, string iconPath = null, string forceAppId = null, string forceAppSecret = null)
        {
            if (!string.IsNullOrWhiteSpace(forceAppId) && !string.IsNullOrWhiteSpace(forceAppSecret))
            {
                this.AppId = forceAppId;
                this.AppSecret = forceAppSecret;
            }
            else
            {
                this.AppId = Guid.NewGuid().ToString("N");
                this.AppSecret = Guid.NewGuid().ToString("N");
            }
            this.AppName = name;
            this.AppDescription = description;
            this.AppCategory = category;
            this.AppPlatform = platform;
            this.IconPath = iconPath;
        }
        public string AppId { get; set; }
        [JsonIgnore]
        public string AppSecret { get; set; }
        public string AppName { get; set; }

        /// <summary>
        /// sitename/filepath/filename.extension
        /// </summary>
        public string IconPath { get; set; }
        public string AppDescription { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy - MM - dd}")]
        public DateTime AppCreateTime { get; set; } = DateTime.UtcNow;

        public Category AppCategory { get; set; }
        public Platform AppPlatform { get; set; }

        public bool EnableOAuth { get; set; } = true;
        /// <summary>
        /// Force the user to input his password even when he is already signed in.
        /// </summary>
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

        public string CreatorId { get; set; }
        [ForeignKey(nameof(CreatorId))]
        [JsonIgnore]
        public DeveloperUser Creator { get; set; }

        [Url]
        [Display(Name = "Privacy Statement Url")]
        public string PrivacyStatementUrl { get; set; }
        [Url]
        [Display(Name = "License Url")]
        public string LicenseUrl { get; set; }
    }
}
