using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Models
{
    public class Site
    {
        public int Id { get; set; }

        public string AppId { get; set; }
        [ForeignKey(nameof(AppId))]
        public ProbeApp Context { get; set; }

        public int FolderId { get; set; }
        [ForeignKey(nameof(FolderId))]
        public Folder Root { get; set; }
    }
}
