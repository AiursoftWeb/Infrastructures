using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.Pylon.Models.OSS
{
    public class Bucket
    {
        public int BucketId { get; set; }
        public string BucketName { get; set; }
        public bool OpenToRead { get; set; } = true;
        public bool OpenToUpload { get; set; } = false;

        public string BelongingAppId { get; set; }
        [JsonIgnore]
        [ForeignKey(nameof(BelongingAppId))]
        public OSSApp BelongingApp { get; set; }
        [JsonIgnore]
        [InverseProperty(nameof(OSSFile.BelongingBucket))]
        public List<OSSFile> Files { get; set; }
        [NotMapped]
        public int FileCount { get; set; }
    }
}
