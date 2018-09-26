using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.OSS.Models.DownloadAddressModels
{
    public abstract class GetFileAddressModel
    {
        public string SD { get; set; } = string.Empty;
        [Range(-1, 10000)]
        public int W { get; set; } = -1;
        [Range(-1, 10000)]
        public int H { get; set; } = -1;
    }
    public class DownloadFileAddressModel : GetFileAddressModel
    {
        public string BucketName { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }

    }
    public class FromSecretAddressModel : GetFileAddressModel
    {
        [Required]
        public string Sec { get; set; }
    }
    public class FromKeyAddressModel : GetFileAddressModel
    {
        [Required]
        public int Id { get; set; }
    }
}
