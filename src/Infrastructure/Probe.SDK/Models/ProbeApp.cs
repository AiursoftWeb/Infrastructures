using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.Probe.SDK.Models;

public class ProbeApp
{
    [Key] public string AppId { get; set; }

    [InverseProperty(nameof(Site.Context))]
    public IEnumerable<Site> Sites { get; set; }
}