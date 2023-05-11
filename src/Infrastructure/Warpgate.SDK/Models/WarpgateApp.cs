using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.Warpgate.SDK.Models;

public class WarpgateApp
{
    [Key] public string AppId { get; set; }

    [InverseProperty(nameof(WarpRecord.App))]
    public IEnumerable<WarpRecord> WarpRecords { get; set; }
}