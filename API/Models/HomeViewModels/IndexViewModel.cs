using Aiursoft.Pylon.Models;
using System;

namespace Aiursoft.API.Models.HomeViewModels
{
    public class IndexViewModel : AiurProtocol
    {
        public DateTime ServerTime { get; internal set; }
        public bool Signedin { get; internal set; }
        public string Local { get; internal set; }
        public APIUser User { get; set; }
    }
}
