using Aiursoft.Developer.SDK.Models;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.Developer.Models
{
    public class DeveloperApp : App
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public DeveloperApp() : base() { }
        public DeveloperApp(string name, string description, Category category, Platform platform, string iconPath = null, string forceAppId = null, string forceAppSecret = null)
            : base(name, description, category, platform, iconPath, forceAppId, forceAppSecret) { }

        public string CreatorId { get; set; }
        [ForeignKey(nameof(CreatorId))]
        [JsonIgnore]
        public DeveloperUser Creator { get; set; }
    }
}
