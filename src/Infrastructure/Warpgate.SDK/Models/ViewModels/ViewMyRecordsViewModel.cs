using System.Collections.Generic;
using Aiursoft.Handler.Models;

namespace Aiursoft.Warpgate.SDK.Models.ViewModels;

public class ViewMyRecordsViewModel : AiurProtocol
{
    public string AppId { get; set; }
    public List<WarpRecord> Records { get; set; }
}