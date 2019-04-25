using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

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
    }
}
