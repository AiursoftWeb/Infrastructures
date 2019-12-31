using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.SDK.Models.Developer.ApiAddressModels
{
    public class AppInfoAddressModel
    {
        [Required]
        [FromQuery(Name = "appid")]
        public string AppId { get; set; }
    }
}
