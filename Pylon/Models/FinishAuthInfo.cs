using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models
{
    public class FinishAuthInfo
    {
        [Required]
        [FromQuery(Name = "appid")]
        public string AppId { get; set; }

        [Url]
        [Required]
        [FromQuery(Name = "redirect_uri")]
        public string RedirectUri { get; set; }

        [FromQuery(Name = "state")]
        public string State { get; set; }
    }
}
