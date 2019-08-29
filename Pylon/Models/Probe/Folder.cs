using Aiursoft.Pylon.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.Pylon.Models.Probe
{
    public class Folder
    {
        [JsonIgnore]
        public int Id { get; set; }

        [JsonIgnore]
        public int? ContextId { get; set; }

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
}
