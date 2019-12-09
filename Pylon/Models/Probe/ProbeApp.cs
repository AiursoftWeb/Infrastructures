using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.Pylon.Models.Probe
{
    public class ProbeApp
    {
        [Key]
        public string AppId { get; set; }

        [InverseProperty(nameof(Site.Context))]
        public IEnumerable<Site> Sites { get; set; }
    }
}
