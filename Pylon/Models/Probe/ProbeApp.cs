using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Aiursoft.Pylon.Models.Probe
{
    public class ProbeApp
    {
        [Key]
        public virtual string AppId { get; set; }

        [InverseProperty(nameof(Site.Context))]
        public IEnumerable<Site> Sites { get; set; }
    }
}
