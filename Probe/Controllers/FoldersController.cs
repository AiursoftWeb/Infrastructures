using Aiursoft.Pylon.Attributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    public class FoldersController : Controller
    {
        public FoldersController()
        {

        }
        [Route("Folders/ViewContent/{siteName}/{**folderNames}")]
        public async Task<IActionResult> ViewContent(string siteName, params string[] folderNames)
        {
            throw new NotImplementedException();
        }
    }
}
