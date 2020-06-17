using Aiursoft.SDKTools.Attributes;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.Wrapgate.SDK.Models
{
    public enum RecordType
    {
        /// <summary>
        /// Support HTTP, HTTPS, IP.
        /// </summary>
        Redirect,
        /// <summary>
        /// Support HTTP, HTTPS, IP
        /// </summary>
        PermanentRedirect,
        /// <summary>
        /// Support HTTPS
        /// </summary>
        IFrame,
        /// <summary>
        /// Support HTTP, HTTPS, IP
        /// </summary>
        ReverseProxy
    }
    public class WrapRecord
    {
        [Key]
        public int Id { get; set; }
        public string AppId { get; set; }
        [ForeignKey(nameof(AppId))]
        [JsonIgnore]
        public WrapgateApp App { get; set; }

        [ValidDomainName]
        public string RecordUniqueName { get; set; }
        [Url]
        public string TargetUrl { get; set; }
        public RecordType Type { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;

        public bool Enabled { get; set; } = true;
        //public bool Recursive { get; set; } = true;
    }
}
