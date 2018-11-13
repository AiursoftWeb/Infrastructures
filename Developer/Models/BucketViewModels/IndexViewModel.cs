using Aiursoft.Pylon.Models.Developer;
using Aiursoft.Pylon.Models.OSS;
using Aiursoft.Developer.Models.AppsViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Models.BucketViewModels
{
    public class IndexViewModel : AppLayoutModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public IndexViewModel() { }
        public IndexViewModel(DeveloperUser user) : base(user, 2) { }
        public IEnumerable<IGrouping<string, Bucket>> AllBuckets { get; set; }
    }
}
