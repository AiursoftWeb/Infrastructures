using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.Probe.SDK.Models
{
    [Index(nameof(HardwareId), IsUnique = true)]
    public class File
    {
        [JsonIgnore]
        public int Id { get; set; }

        public int ContextId { get; set; }
        [JsonIgnore]
        [ForeignKey(nameof(ContextId))]
        public Folder Context { get; set; }

        public string FileName { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadTime { get; set; } = DateTime.UtcNow;
        [JsonIgnore]
        public string HardwareId { get; set; } = Guid.NewGuid().ToString();
    }
}