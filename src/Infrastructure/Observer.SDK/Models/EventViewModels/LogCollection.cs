namespace Aiursoft.Observer.SDK.Models.EventViewModels;

public class LogCollection
{
    public string Message { get; set; }
    public ErrorLog First { get; set; }
    public int Count { get; set; }
}