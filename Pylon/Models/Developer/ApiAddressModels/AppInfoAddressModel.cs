using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.Developer.ApiAddressModels
{
    public class AppInfoAddressModel
    {
        [Required]
        [FromQuery(Name = "appId")]
        public string AppId { get; set; }
    }
}
