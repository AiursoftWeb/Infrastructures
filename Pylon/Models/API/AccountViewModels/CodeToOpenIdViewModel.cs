using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Models.API.AccountViewModels
{
    public class CodeToOpenIdViewModel : AiurProtocol
    {
        public string OpenId { get; set; }
        public string Scope { get; set; }
    }
}
