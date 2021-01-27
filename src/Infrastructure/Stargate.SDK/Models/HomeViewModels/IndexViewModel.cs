using Aiursoft.Handler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiursoft.Stargate.SDK.Models.HomeViewModels
{
    public class IndexViewModel : AiurProtocol
    {
        public int CurrentId { get; set; }
        public int TotalMemoryMessages { get; set; }
        public int Channels { get; set; }
    }
}
