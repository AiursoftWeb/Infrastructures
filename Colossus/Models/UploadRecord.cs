using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Colossus.Models
{
    public class UploadRecord
    {
        public int Id { get; set; }
        public string UploaderId { get; set; }
        [ForeignKey(nameof(UploaderId))]
        public ColossusUser Uploader { get; set; }
        public string SourceFileName { get; set; }
        public int FileId { get; set; }
        public DateTime UploadTime { get; set; } = DateTime.UtcNow;
    }
}
