using Aiursoft.Probe.Data;
using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Probe;
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

        public FolderLocator(ProbeDbContext dbContext)
        {
            _dbContext = dbContext;
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
                    ?? throw new AiurAPIModelException(ErrorType.NotFound, $"Not found folder {folder} under folder {currentFolder.FolderName}!");
            }
            return currentFolder;
        }
    }
}
