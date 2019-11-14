using System.Collections.Generic;

namespace Aiursoft.Pylon.Models.Status.EventViewModels
{
    public class ViewLogViewModel : AiurProtocol
    {
        public List<LogCollection> Logs { get; set; }
        public string AppId { get; set; }
    }

    public class LogCollection
    {
        public string Message { get; set; }
        public ErrorLog First { get; set; }
        public int Count { get; set; }
    }
}
