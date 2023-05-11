using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Aiursoft.Probe.SDK.Models;

[Index(nameof(SiteName), IsUnique = true)]
public class Site
{
    [JsonIgnore] public int Id { get; set; }

    public string AppId { get; set; }

    [ForeignKey(nameof(AppId))]
    [JsonIgnore]
    public ProbeApp Context { get; set; }

    [JsonIgnore] public int RootFolderId { get; set; }

    [ForeignKey(nameof(RootFolderId))]
    [JsonIgnore]
    public Folder Root { get; set; }

    public string SiteName { get; set; }
    public bool OpenToUpload { get; set; }
    public bool OpenToDownload { get; set; }

    public DateTime CreationTime { get; set; } = DateTime.UtcNow;
}