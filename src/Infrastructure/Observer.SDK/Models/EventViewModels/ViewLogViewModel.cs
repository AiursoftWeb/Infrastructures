using Aiursoft.Handler.Models;
using System.Collections.Generic;

namespace Aiursoft.Observer.SDK.Models.EventViewModels
{
    public class ViewLogViewModel : AiurProtocol
    {
        public List<LogCollection> Logs { get; set; }
        public string AppId { get; set; }
    }
}
