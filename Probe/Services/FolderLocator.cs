using Aiursoft.Probe.Data;
using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Probe;
using Aiursoft.Pylon.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Services
{
    public class FolderLocator
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

        public async Task<Folder> LocateSiteAndFolder(string accessToken, string siteName, string folderNames)
        {
            var appid = _tokenManager.ValidateAccessToken(accessToken);
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
            var folder = await LocateAsync(folderNames, site.Root);
            return folder;
        }

        public async Task<Folder> LocateAsync(string folderNames, Folder root)
        {
            string[] folders = folderNames?.Split('/', StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
            var currentFolder = root;
            foreach (var folder in folders)
            {
                var folderObject = await _dbContext
                    .Folders
                    .Include(t => t.SubFolders)
                    .Include(t => t.Files)
                    .Where(t => t.ContextId == currentFolder.Id)
                    .SingleOrDefaultAsync(t => t.FolderName == folder.ToLower());
                currentFolder = folderObject
                    ?? throw new AiurAPIModelException(ErrorType.NotFound, $"Not found folder '{folder}' under folder '{currentFolder.FolderName}'!");
            }
            return currentFolder;
        }
    }
}
