using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Aiursoft.Pylon.Models.Developer
{
    public class DeveloperUser : AiurUserBase
    {
        [InverseProperty(nameof(App.Creator))]
        public virtual List<App> MyApps { get; set; } = new List<App>();
    }
}
