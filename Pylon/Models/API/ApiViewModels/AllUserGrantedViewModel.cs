using System.Collections.Generic;

namespace Aiursoft.Pylon.Models.API.ApiViewModels
{
    public class AllUserGrantedViewModel : AiurProtocol
    {
        public string AppId { get; set; }
        public List<Grant> Grants { get; set; }
    }
}
