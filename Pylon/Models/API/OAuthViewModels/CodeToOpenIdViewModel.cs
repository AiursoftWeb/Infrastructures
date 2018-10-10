using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Models.API.OAuthViewModels
{
    public class CodeToOpenIdViewModel : AiurProtocal
    {
        public string openid { get; set; }
        public string scope { get; set; }
    }
}
