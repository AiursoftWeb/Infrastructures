using Aiursoft.SDK.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.Developer.Models
{
    public class DeveloperUser : AiurUserBase
    {
        [InverseProperty(nameof(DeveloperApp.Creator))]
        public virtual List<DeveloperApp> MyApps { get; set; } = new List<DeveloperApp>();
    }
}
