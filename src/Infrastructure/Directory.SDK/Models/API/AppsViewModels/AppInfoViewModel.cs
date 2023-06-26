using Aiursoft.AiurProtocol;

namespace Aiursoft.Directory.SDK.Models.API.AppsViewModels;

public class AppInfoViewModel : AiurResponse
{
    public DirectoryApp App { get; set; }
}