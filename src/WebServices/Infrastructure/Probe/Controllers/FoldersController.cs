using System.Threading.Tasks;
using Aiursoft.Probe.Repositories;
using Aiursoft.Probe.SDK.Models;
using Aiursoft.Probe.SDK.Models.FoldersAddressModels;
using Aiursoft.Probe.Services;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.AiurProtocol;
using Aiursoft.AiurProtocol.Server;

namespace Aiursoft.Probe.Controllers;

[ApiExceptionHandler]
[ApiModelStateChecker]
[Route("Folders")]
public class FoldersController : ControllerBase
{
    private readonly FolderRepo _folderRepo;
    private readonly FolderSplitter _folderSplitter;

    public FoldersController(
        FolderSplitter folderLocator,
        FolderRepo folderRepo)
    {
        _folderSplitter = folderLocator;
        _folderRepo = folderRepo;
    }

    [Route("ViewContent/{SiteName}/{**FolderNames}")]
    [Produces(typeof(AiurValue<Folder>))]
    public async Task<IActionResult> ViewContent(ViewContentAddressModel model)
    {
        var folders = _folderSplitter.SplitToFolders(model.FolderNames);
        var folder = await _folderRepo.GetFolderAsOwner(model.AccessToken, model.SiteName, folders);
        if (folder == null)
        {
            return this.Protocol(Code.NotFound, "Locate folder failed!");
        }

        return this.Protocol(new AiurValue<Folder>(folder)
        {
            Code = Code.ResultShown,
            Message = "Successfully get your folder!"
        });
    }

    [HttpPost]
    [Route("CreateNewFolder/{SiteName}/{**FolderNames}")]
    public async Task<IActionResult> CreateNewFolder(CreateNewFolderAddressModel model)
    {
        var folders = _folderSplitter.SplitToFolders(model.FolderNames);
        var folder =
            await _folderRepo.GetFolderAsOwner(model.AccessToken, model.SiteName, folders, model.RecursiveCreate);
        if (folder == null)
        {
            return this.Protocol(Code.NotFound, "Locate folder failed!");
        }

        await _folderRepo.CreateNewFolder(folder.Id, model.NewFolderName);
        return this.Protocol(Code.JobDone, "Successfully created your new folder!");
    }

    [HttpPost]
    [Route("DeleteFolder/{SiteName}/{**FolderNames}")]
    public async Task<IActionResult> DeleteFolder(DeleteFolderAddressModel model)
    {
        var folders = _folderSplitter.SplitToFolders(model.FolderNames);
        var folder = await _folderRepo.GetFolderAsOwner(model.AccessToken, model.SiteName, folders);
        if (folder == null)
        {
            return this.Protocol(Code.NotFound, "Locate folder failed!");
        }

        if (folder.ContextId == null)
        {
            return this.Protocol(Code.InvalidInput,
                "We can not delete root folder! If you wanna delete your site, please consider delete your site directly!");
        }

        await _folderRepo.DeleteFolder(folder.Id);
        return this.Protocol(Code.JobDone, "Successfully deleted your folder!");
    }
}