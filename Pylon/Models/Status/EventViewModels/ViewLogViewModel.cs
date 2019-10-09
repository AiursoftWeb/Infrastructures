using System.Collections.Generic;

namespace Aiursoft.Pylon.Models.Status.EventViewModels
{
    public class ViewLogViewModel : AiurProtocol
    {
        public List<ErrorLog> Logs { get; set; }
        public string AppId { get; set; }
    }
}
