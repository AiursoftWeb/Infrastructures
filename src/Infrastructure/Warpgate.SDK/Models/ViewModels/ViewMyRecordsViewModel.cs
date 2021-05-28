using Aiursoft.Handler.Models;
using System.Collections.Generic;

namespace Aiursoft.Warpgate.SDK.Models.ViewModels
{
    public class ViewMyRecordsViewModel : AiurProtocol
    {
        public string AppId { get; set; }
        public List<WarpRecord> Records { get; set; }
    }
}
