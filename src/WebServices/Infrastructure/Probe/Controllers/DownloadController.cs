﻿using Aiursoft.AiurProtocol.Exceptions;
using Aiursoft.AiurProtocol.Models;
using Aiursoft.AiurProtocol.Server.Attributes;
using Aiursoft.Probe.Repositories;
using Aiursoft.Probe.SDK.Models.DownloadAddressModels;
using Aiursoft.Probe.SDK.Services;
using Aiursoft.Probe.Services;
using Aiursoft.Probe.ViewModels.DownloadViewModels;
using Aiursoft.CSTools.Tools;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Probe.Controllers;

[Route("Download")]
[ApiExceptionHandler]
public class DownloadController : Controller
{
    private readonly FileRepo _fileRepo;
    private readonly FolderRepo _folderRepo;
    private readonly FolderSplitter _folderSplitter;
    private readonly ImageCompressor _imageCompressor;
    private readonly SiteRepo _siteRepo;
    private readonly ILogger<DownloadController> _logger;
    private readonly IStorageProvider _storageProvider;
    private readonly TokenEnsurer _tokenEnsurer;
    private readonly ProbeSettingsFetcher _probeLocator;

    public DownloadController(
        FolderSplitter folderLocator,
        ImageCompressor imageCompressor,
        TokenEnsurer tokenEnsurer,
        IStorageProvider storageProvider,
        FolderRepo folderRepo,
        FileRepo fileRepo,
        SiteRepo siteRepo,
        ILogger<DownloadController> logger,
        ProbeSettingsFetcher probeLocator)
    {
        _folderSplitter = folderLocator;
        _imageCompressor = imageCompressor;
        _tokenEnsurer = tokenEnsurer;
        _storageProvider = storageProvider;
        _folderRepo = folderRepo;
        _fileRepo = fileRepo;
        _siteRepo = siteRepo;
        _logger = logger;
        _probeLocator = probeLocator;
    }

    [Route("File/{SiteName}/{**FolderNames}", Name = "File")]
    [Route("Open/{SiteName}/{**FolderNames}", Name = "Open")]
    [Route("Video/{SiteName}/{**FolderNames}", Name = "Video")]
    public async Task<IActionResult> Open(OpenAddressModel model)
    {
        var site = await _siteRepo.GetSiteByName(model.SiteName);
        if (site == null)
        {
            return NotFound();
        }

        if (!site.OpenToDownload)
        {
            _tokenEnsurer.Ensure(model.PBToken, "Download", model.SiteName, model.FolderNames);
        }

        var (folders, fileName) = _folderSplitter.SplitToFoldersAndFile(model.FolderNames);
        try
        {
            var siteRoot = await _folderRepo.GetFolderFromId(site.RootFolderId);
            var folder = await _folderRepo.GetFolderFromPath(folders, siteRoot, false);
            if (folder == null)
            {
                return NotFound();
            }

            var file = await _fileRepo.GetFileInFolder(folder, fileName);
            if (file == null)
            {
                return NotFound();
            }

            var path = _storageProvider.GetFilePath(file.HardwareId);
            var extension = _storageProvider.GetExtension(file.FileName);
            if (ControllerContext.ActionDescriptor.AttributeRouteInfo?.Name == "File")
            {
                return this.WebFile(path, "do-not-open");
            }

            if (ControllerContext.ActionDescriptor.AttributeRouteInfo?.Name == "Video")
            {
                return VideoPlayerWithFile(
                    await _probeLocator.GetProbeOpenAddressAsync(model.SiteName, folders, fileName),
                    model.PBToken,
                    fileName);
            }

            if (file.FileName.IsStaticImage() && await IsValidImageAsync(path))
            {
                return await FileWithImageCompressor(path, extension);
            }

            return this.WebFile(path, extension);
        }
        catch (AiurUnexpectedServerResponseException e) when (e.Response.Code == Code.NotFound)
        {
            return NotFound();
        }
    }

    private async Task<bool> IsValidImageAsync(string imagePath)
    {
        try
        {
            _ = await Image.DetectFormatAsync(imagePath);
            _logger.LogInformation("File with path {ImagePath} is an valid image", imagePath);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "File with path {ImagePath} is not an valid image", imagePath);
            return false;
        }
    }

    private IActionResult VideoPlayerWithFile(string path, string pbToken, string fileName)
    {
        var model = new PlayerViewModel(path, pbToken)
        {
            Title = fileName
        };
        return View("Player", model);
    }

    private async Task<IActionResult> FileWithImageCompressor(string path, string extension)
    {
        int.TryParse(Request.Query["w"], out var width);
        bool.TryParse(Request.Query["square"], out var square);
        if (width > 0)
        {
            if (square)
            {
                return this.WebFile(await _imageCompressor.Compress(path, width, width, extension), extension);
            }

            return this.WebFile(await _imageCompressor.Compress(path, width, 0, extension), extension);
        }

        return this.WebFile(await _imageCompressor.ClearExif(path), extension);
    }
}