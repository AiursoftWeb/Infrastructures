using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Aiursoft.Directory.SDK.Models;

namespace Aiursoft.Developer.Models;

public class DeveloperUser : AiurUserBase
{
    [InverseProperty(nameof(DeveloperApp.Creator))]
    public virtual List<DeveloperApp> MyApps { get; set; } = new();
}