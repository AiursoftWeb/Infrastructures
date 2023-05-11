using System;
using Newtonsoft.Json;

namespace Aiursoft.Observer.SDK.Models;

public class ErrorLog
{
    [JsonIgnore] public int Id { get; set; }

    public string AppId { get; set; }
    public string Message { get; set; }
    public string StackTrace { get; set; }
    public EventLevel EventLevel { get; set; }
    public string Path { get; set; }

    public DateTime LogTime { get; set; } = DateTime.UtcNow;
}