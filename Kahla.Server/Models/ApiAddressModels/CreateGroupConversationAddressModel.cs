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
        [StringLength(maximumLength: 25, MinimumLength = 3)]
        public string GroupName { get; set; }
    }
}
