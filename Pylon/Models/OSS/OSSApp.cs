using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

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
