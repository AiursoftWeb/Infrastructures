using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.Pylon.Models.Probe
{
    public class File
    {
        [JsonIgnore]
        public int Id { get; set; }

        public int ContextId { get; set; }
        [JsonIgnore]
        [ForeignKey(nameof(ContextId))]
        public Folder Context { get; set; }

        public string FileName { get; set; }
        public DateTime UploadTime { get; set; } = DateTime.UtcNow;
    }
}
