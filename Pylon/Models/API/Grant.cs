using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Aiursoft.Pylon.Models.API
{
    public class Grant
    {
        public string AppID { get; set; }
        public DateTime GrantTime { get; set; } = DateTime.UtcNow;
        public string APIUserId { get; set; }
        [NotMapped]
        public virtual AiurUserBase UserInfo { get; set; }
    }
}
