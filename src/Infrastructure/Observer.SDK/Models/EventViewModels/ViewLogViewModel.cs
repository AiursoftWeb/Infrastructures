using System.Collections.Generic;
using Aiursoft.Handler.Models;

namespace Aiursoft.Observer.SDK.Models.EventViewModels;

public class ViewLogViewModel : AiurProtocol
{
    public List<LogCollection> Logs { get; set; }
    public string AppId { get; set; }
}