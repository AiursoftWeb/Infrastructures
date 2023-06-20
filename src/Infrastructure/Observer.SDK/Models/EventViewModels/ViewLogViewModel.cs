using Aiursoft.AiurProtocol.Models;
using System.Collections.Generic;

namespace Aiursoft.Observer.SDK.Models.EventViewModels;

public class ViewLogViewModel : AiurResponse
{
    public List<LogCollection> Logs { get; set; }
    public string AppId { get; set; }
}