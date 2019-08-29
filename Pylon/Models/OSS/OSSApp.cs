using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.Pylon.Models.OSS
{
    public class OSSApp
    {
        [Key]
        public virtual string AppId { get; set; }
        [JsonIgnore]
        [InverseProperty(nameof(Bucket.BelongingApp))]
        public List<Bucket> MyBuckets { get; set; }
    }
}
