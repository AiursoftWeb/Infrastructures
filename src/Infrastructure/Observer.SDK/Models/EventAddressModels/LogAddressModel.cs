using System.ComponentModel.DataAnnotations;
using Aiursoft.Directory.SDK.Attributes;

namespace Aiursoft.Observer.SDK.Models.EventAddressModels;

public class LogAddressModel
{
    [Required] [IsAccessToken] public string AccessToken { get; set; }

    public string Message { get; set; }
    public string StackTrace { get; set; }
    public string Path { get; set; }
    public EventLevel EventLevel { get; set; }
}