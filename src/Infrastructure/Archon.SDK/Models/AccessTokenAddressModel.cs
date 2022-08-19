#nullable enable
using Aiursoft.SDKTools.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Archon.SDK.Models
{
    public class AccessTokenAddressModel
    {
        [Required]
        [IsGuidOrEmpty]
        [FromQuery(Name = "appId")]
        public virtual string? AppId { get; set; }
        [Required]
        [IsGuidOrEmpty]
        [FromQuery(Name = "appSecret")]
        public virtual string? AppSecret { get; set; }
    }
}
