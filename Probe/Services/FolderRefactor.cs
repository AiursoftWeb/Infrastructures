using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Probe.Data;
using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Probe;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Probe.Services
{
    public class FolderRefactor
    {
        private ProbeDbContext _dbContext;
        private FolderLocator _folderLocator;

        public FolderRefactor(ProbeDbContext dbContext, FolderLocator folderLocator)
        {
            _dbContext = dbContext;
            _folderLocator = folderLocator;
        }

        public async Task MoveFolder(string accessToken, string siteName, string[] folderNames, string[] newFolderNames)
        {
            if (folderNames.SequenceEqual(newFolderNames)) return;
            if (newFolderNames.Length == 0 || folderNames.Length == 0)
            {
                throw new AiurAPIModelException(ErrorType.InvalidInput, "Moving the root folder is unsupported.");
            }

            var root = await _folderLocator.LocateSiteAndFolder(accessToken, siteName);
            var folder = await _folderLocator.LocateAsync(folderNames, root, false);
            var newParent = folderNames.Take(folderNames.Length - 1).SequenceEqual(newFolderNames.Take(folderNames.Length - 1))
                ? folder.Context
                : await _folderLocator.LocateAsync(newFolderNames.Take(newFolderNames.Length - 1).ToArray(), root, false);
            if (newParent.SubFolders.Any(t => t.FolderName == newFolderNames.Last()))
            {
                throw new AiurAPIModelException(ErrorType.NotEnoughResources,
                    $"There is already an folder named '{newFolderNames.Last()}' in folder '{newParent.FolderName}'");
            }

            // change folder name
            folder.FolderName = newFolderNames.Last();
            // move the folder
            folder.ContextId = newParent.Id;
        }

        public async Task MoveFile(string accessToken, string siteName, string[] folderNames, string[] newFolderNames)
        {
            if (folderNames.SequenceEqual(newFolderNames)) return;
            if (newFolderNames.Length == 0 || folderNames.Length == 0)
            {
                throw new AiurAPIModelException(ErrorType.InvalidInput, "Moving the root folder is unsupported.");
            }

            var root = await _folderLocator.LocateSiteAndFolder(accessToken, siteName);
            var parent = await _folderLocator.LocateAsync(folderNames.Take(folderNames.Length - 1).ToArray(), root, false);
            var file = parent.Files.SingleOrDefault(t => t.FileName == folderNames.Last());
            if (file == null)
            {
                throw new AiurAPIModelException(ErrorType.NotFound, $"Not found file '{file}' under folder '{parent.FolderName}'!");
            }
            var newParent = folderNames.Take(folderNames.Length - 1).SequenceEqual(newFolderNames.Take(folderNames.Length - 1))
                ? parent
                : await _folderLocator.LocateAsync(newFolderNames.Take(newFolderNames.Length - 1).ToArray(), root, false);
            if (newParent.Files.Any(t => t.FileName == newFolderNames.Last()))
            {
                throw new AiurAPIModelException(ErrorType.NotEnoughResources,
                    $"There is already an file named '{newFolderNames.Last()}' in folder '{newParent.FolderName}'");
            }

            // change folder name
            file.FileName = newFolderNames.Last();
            // move the folder
            file.ContextId = newParent.Id;
        }
    }
}
