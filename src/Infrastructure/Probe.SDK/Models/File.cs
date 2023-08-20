using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Aiursoft.Probe.SDK.Models;

public class File
{
    [JsonIgnore] public int Id { get; set; }

    [JsonIgnore] public int ContextId { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(ContextId))]
    public Folder Context { get; set; }

    public string FileName { get; set; }
    public long FileSize { get; set; }
    public DateTime UploadTime { get; set; } = DateTime.UtcNow;

    [JsonIgnore] public string HardwareId { get; set; } = Guid.NewGuid().ToString();
}