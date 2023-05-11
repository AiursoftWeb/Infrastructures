using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Observer.SDK.Models.EventAddressModels;

public class LogAddressModel
{
    [Required] public string AccessToken { get; set; }

    public string Message { get; set; }
    public string StackTrace { get; set; }
    public string Path { get; set; }
    public EventLevel EventLevel { get; set; }
}