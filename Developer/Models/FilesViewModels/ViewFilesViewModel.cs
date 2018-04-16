using Aiursoft.Pylon;
using Aiursoft.Pylon.Models.Developer;
using Aiursoft.Pylon.Models.OSS;
using Aiursoft.Pylon.Models.OSS.ApiViewModels;
using Aiursoft.Developer.Models.AppsViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Models.FilesViewModels
{
    public class ViewFilesViewModel : AppLayoutModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public ViewFilesViewModel() { }
        public ViewFilesViewModel(DeveloperUser User) : base(User, 3) { }
        public virtual int BucketId { get; set; }
        public virtual string BucketName { get; set; }
        public IEnumerable<OSSFile> AllFiles { get; set; }
        public virtual string AppId { get; set; }
        public virtual bool OpenToRead { get; set; }
    }
}
