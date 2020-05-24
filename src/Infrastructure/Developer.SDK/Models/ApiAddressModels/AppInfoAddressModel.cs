using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Developer.SDK.Models.ApiAddressModels
{
    public class AppInfoAddressModel
    {
        [Required]
        [FromQuery(Name = "appid")]
        public string AppId { get; set; }
    }
}
