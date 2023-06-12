using System;
using System.ComponentModel.DataAnnotations.Schema;
using Aiursoft.Directory.SDK.Models;
using Newtonsoft.Json;

namespace Aiursoft.Directory.Models;

public class DirectoryAppInDb : DirectoryApp
{
    [Obsolete(error: true, message: "This method is only for framework!")]
    public DirectoryAppInDb()
    {
    }

    public DirectoryAppInDb(string name, string description, string iconPath)
        : base(name, description, iconPath)
    {
    }

    public string CreatorId { get; set; }

    [ForeignKey(nameof(CreatorId))]
    [JsonIgnore]
    public DirectoryUser Creator { get; set; }
}