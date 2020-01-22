using Aiursoft.XelNaga.Models; using Aiursoft.Handler.Models;
using System;

namespace Aiursoft.Gateway.Models.HomeViewModels
{
    public class IndexViewModel : AiurProtocol
    {
        public DateTime ServerTime { get; internal set; }
        public bool Signedin { get; internal set; }
        public string Local { get; internal set; }
        public GatewayUser User { get; set; }
    }
}
