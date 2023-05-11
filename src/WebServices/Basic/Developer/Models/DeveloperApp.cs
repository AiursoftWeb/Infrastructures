using System;
using System.ComponentModel.DataAnnotations.Schema;
using Aiursoft.Developer.SDK.Models;
using Newtonsoft.Json;

namespace Aiursoft.Developer.Models;

public class DeveloperApp : App
{
    [Obsolete(error: true, message: "This method is only for framework!")]
    public DeveloperApp()
    {
    }

    public DeveloperApp(string name, string description, Category category, Platform platform, string iconPath = null)
        : base(name, description, category, platform, iconPath)
    {
    }

    public string CreatorId { get; set; }

    [ForeignKey(nameof(CreatorId))]
    [JsonIgnore]
    public DeveloperUser Creator { get; set; }
}