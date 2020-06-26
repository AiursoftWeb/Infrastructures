using Aiursoft.Handler.Models;
using System;

namespace Aiursoft.Gateway.Models.HomeViewModels
{
    public class IndexViewModel : AiurProtocol
    {
        public DateTime ServerTime { get; internal set; }
        public bool SignedIn { get; internal set; }
        public string Local { get; internal set; }
        public GatewayUser User { get; set; }
    }
}
