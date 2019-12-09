using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.Pylon.Models.Probe
{
    public class Site
    {
        [JsonIgnore]
        public int Id { get; set; }

        public string AppId { get; set; }
        [ForeignKey(nameof(AppId))]
        [JsonIgnore]
        public ProbeApp Context { get; set; }

        [JsonIgnore]
        public int FolderId { get; set; }
        [ForeignKey(nameof(FolderId))]
        [JsonIgnore]
        public Folder Root { get; set; }

        public string SiteName { get; set; }
        public bool OpenToUpload { get; set; }
        public bool OpenToDownload { get; set; }

        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
    }
}
