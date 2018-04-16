using Aiursoft.Pylon.Models.Developer;
using Aiursoft.Pylon.Models.OSS;
using Aiursoft.Pylon.Models.OSS.ApiViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Models.BucketViewModels
{
    public class EditBucketViewModel : CreateBucketViewModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public EditBucketViewModel() { }
        public EditBucketViewModel(DeveloperUser User, ViewBucketViewModel thisBucket) : base(User)
        {
            this.NewBucketName = thisBucket.BucketName;
            this.BucketId = thisBucket.BucketId;
            this.OpenToRead = thisBucket.OpenToRead;
            this.OpenToUpload = thisBucket.OpenToUpload;
        }
        [Required]
        public int BucketId { get; set; }
    }
}
