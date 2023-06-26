using System.Collections.Generic;
using Aiursoft.AiurProtocol;

namespace Aiursoft.Warpgate.SDK.Models.ViewModels;

public class ViewMyRecordsViewModel : AiurResponse
{
    public string AppId { get; set; }
    public List<WarpRecord> Records { get; set; }
}