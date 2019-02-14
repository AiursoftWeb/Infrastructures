using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Aiursoft.Pylon.Models.OSS
{
    public class Secret
    {
        public int Id { get; set; }
        public string Value { get; set; }

        public int FileId { get; set; }
        [ForeignKey(nameof(FileId))]
        public OSSFile File { get; set; }

        public int UsedTimes { get; set; } = 0;
        public int MaxUseTime { get; set; }
        public DateTime UseTime { get; set; } = DateTime.MinValue;
        public string UserIpAddress { get; set; } = string.Empty;
    }
}
