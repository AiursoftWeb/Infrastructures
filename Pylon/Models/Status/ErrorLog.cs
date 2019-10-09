using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.Pylon.Models.Status
{
    public enum EventLevel
    {
        Exception = 1,
        Warning = 2,
        Info = 3
    }

    public class ErrorLog
    {
        [JsonIgnore]
        public int Id { get; set; }

        public string AppId { get; set; }
        [ForeignKey(nameof(AppId))]
        [JsonIgnore]
        public StatusApp Context { get; set; }

        public string Message { get; set; }
        public string StackTrace { get; set; }
        public EventLevel EventLevel { get; set; }

        public DateTime LogTime { get; set; } = DateTime.UtcNow;
    }
}
