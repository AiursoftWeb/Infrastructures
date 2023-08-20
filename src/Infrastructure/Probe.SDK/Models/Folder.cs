using System.ComponentModel.DataAnnotations.Schema;
using Aiursoft.DocGenerator.Attributes;
using Newtonsoft.Json;

namespace Aiursoft.Probe.SDK.Models;

public class Folder
{
    [JsonIgnore] public int Id { get; set; }

    [JsonIgnore] public int? ContextId { get; set; }

    [ForeignKey(nameof(ContextId))]
    [JsonIgnore]
    public Folder Context { get; set; }

    [InstanceMakerIgnore]
    [InverseProperty(nameof(Context))]
    public IEnumerable<Folder> SubFolders { get; set; }

    [InverseProperty(nameof(File.Context))]
    public IEnumerable<File> Files { get; set; }

    public string FolderName { get; set; } = "root";
}