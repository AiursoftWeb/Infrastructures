using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Pylon.Attributes;

namespace Kahla.Server.Models.ApiAddressModels
{
    public class CreateGroupConversationAddressModel
    {
        [Required]
        [NoSpace]
        [Range(5, 25)]
        public string GroupName { get; set; }
    }
}
