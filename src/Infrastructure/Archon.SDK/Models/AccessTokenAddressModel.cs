using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Archon.SDK.Models
{
    public class AccessTokenAddressModel
    {
        [Required]
        [FromQuery(Name = "appId")]
        public virtual string AppId { get; set; }
        [Required]
        [FromQuery(Name = "appSecret")]
        public virtual string AppSecret { get; set; }
    }
}
