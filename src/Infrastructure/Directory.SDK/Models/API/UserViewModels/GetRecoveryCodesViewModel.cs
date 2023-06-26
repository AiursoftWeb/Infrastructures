using Aiursoft.AiurProtocol;

namespace Aiursoft.Directory.SDK.Models.API.UserViewModels;

public class GetRecoveryCodesViewModel : AiurResponse
{
    public string RecoveryCodesKey { get; set; }
}