using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Models
{
    public class Folder
    {
        public int Id { get; set; }

        public int? ContextId { get; set; }
        [ForeignKey(nameof(ContextId))]
        public Folder Context { get; set; }

        [InverseProperty(nameof(Context))]
        public IEnumerable<Folder> SubFolders { get; set; }

        [InverseProperty(nameof(File.Context))]
        public IEnumerable<File> Files { get; set; }
    }
}
