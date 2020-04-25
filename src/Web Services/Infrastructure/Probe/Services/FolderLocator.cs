using Aiursoft.Archon.SDK.Services;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Probe.Data;
using Aiursoft.Probe.SDK.Models;
using Aiursoft.Scanner.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Services
{
    public class FolderLocator : ITransientDependency
    {
        private readonly ProbeDbContext _dbContext;
        private readonly ACTokenManager _tokenManager;

        public FolderLocator(
            ProbeDbContext dbContext,
            ACTokenManager tokenManager)
        {
            _dbContext = dbContext;
            _tokenManager = tokenManager;
        }

        public string[] SplitStrings(string folderNames) =>
            folderNames?.Split('/', StringSplitOptions.RemoveEmptyEntries) ?? new string[0];

        public (string[] folders, string fileName) SplitToPath(string folderNames)
        {
            if (folderNames == null || folderNames.Length == 0)
            {
                throw new AiurAPIModelException(ErrorType.NotFound, "The root folder isn't a file!");
            }
            var foldersWithFileName = SplitStrings(folderNames);
            var fileName = foldersWithFileName.Last();
            var folders = foldersWithFileName.Take(foldersWithFileName.Count() - 1).ToArray();
            return (folders, fileName);
        }

        public async Task<Folder> LocateSiteAndFolder(string accessToken, string siteName, string[] folderNames = null, bool recursiveCreate = false)
        {
            var appid = await _tokenManager.ValidateAccessToken(accessToken);
            var site = await _dbContext
                .Sites
                .Include(t => t.Root)
                .Include(t => t.Root.SubFolders)
                .Include(t => t.Root.Files)
                .SingleOrDefaultAsync(t => t.SiteName.ToLower() == siteName.ToLower());
            if (site == null)
            {
                throw new AiurAPIModelException(ErrorType.NotFound, "Not found target site!");
            }
            if (site.AppId != appid)
            {
                throw new AiurAPIModelException(ErrorType.Unauthorized, "The target folder is not your app's folder!");
            }

            if (folderNames == null || folderNames.Length == 0)
            {
                return site.Root;
            }
            var folder = await LocateAsync(folderNames, site.Root, recursiveCreate);
            return folder;
        }

        public async Task<Folder> LocateSiteAndFolder(string siteName, string[] folderNames = null, bool recursiveCreate = false)
        {
            var site = await _dbContext
                .Sites
                .Include(t => t.Root)
                .Include(t => t.Root.SubFolders)
                .Include(t => t.Root.Files)
                .SingleOrDefaultAsync(t => t.SiteName.ToLower() == siteName.ToLower());
            if (site == null)
            {
                throw new AiurAPIModelException(ErrorType.NotFound, "Not found target site!");
            }
            if (folderNames == null || folderNames.Length == 0)
            {
                return site.Root;
            }
            var folder = await LocateAsync(folderNames, site.Root, recursiveCreate);
            return folder;
        }

        public async Task<Folder> LocateAsync(string[] folderNames, Folder root, bool recursiveCreate)
        {
            if (!folderNames.Any())
            {
                return root;
            }
            await _dbContext.Entry(root)
                .Collection(t => t.SubFolders)
                .LoadAsync();
            var subFolderName = folderNames[0];
            var subFolder = root
                .SubFolders
                .SingleOrDefault(t => t.FolderName == subFolderName);
            if (recursiveCreate && subFolder == null && !string.IsNullOrWhiteSpace(subFolderName))
            {
                subFolder = new Folder
                {
                    ContextId = root.Id,
                    FolderName = subFolderName
                };
                _dbContext.Folders.Add(subFolder);
                await _dbContext.SaveChangesAsync();
            }
            return await LocateAsync(
                folderNames.Skip(1).ToArray(), subFolder, recursiveCreate);
        }

        public async Task<Folder> LocateAsyncOBS(string[] folderNames, Folder root, bool recursiveCreate)
        {
            return null;
            //var currentFolder = root;
            //foreach (var folder in folderNames)
            //{
            //    var folderObject = await _dbContext
            //        .Folders
            //        .Include(t => t.SubFolders)
            //        .Include(t => t.Files)
            //        .Include(t => t.Context)
            //        .Where(t => t.ContextId == currentFolder.Id)
            //        .SingleOrDefaultAsync(t => t.FolderName == folder.ToLower());
            //    if (recursiveCreate && folderObject == null && !string.IsNullOrWhiteSpace(folder))
            //    {
            //        folderObject = new Folder
            //        {
            //            ContextId = currentFolder.Id,
            //            FolderName = folder,
            //            Files = new List<File>(),
            //            SubFolders = new List<Folder>()
            //        };
            //        _dbContext.Folders.Add(folderObject);
            //        await _dbContext.SaveChangesAsync();
            //    }
            //    currentFolder = folderObject
            //        ?? throw new AiurAPIModelException(ErrorType.NotFound, $"Not found folder '{folder}' under folder '{currentFolder.FolderName}'!");
            //}
            //return currentFolder;
        }
    }
}
