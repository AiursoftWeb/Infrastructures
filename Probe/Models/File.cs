using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Models
{
    public partial class File
    {
        public int Id { get; set; }

        public int ContextId { get; set; }
        [ForeignKey(nameof(ContextId))]
        public Folder Context { get; set; }
    }

    public partial class File
    {
        public int FileName { get; set; }
        public DateTime UploadTime { get; set; }
    }
}
