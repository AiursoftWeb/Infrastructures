using Aiursoft.Pylon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.API.Models.HomeViewModels
{
    public class IndexViewModel : AiurProtocal
    {
        public DateTime ServerTime { get; internal set; }
        public bool Signedin { get; internal set; }
        public string Local { get; internal set; }
        public APIUser User { get; set; }
    }
}
