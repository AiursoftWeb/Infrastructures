using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Models
{
    public class ProbeApp
    {
        [Key]
        public virtual string AppId { get; set; }

        [InverseProperty(nameof(Site.Context))]
        public IEnumerable<Site> Sites { get; set; }
    }
}
