using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Scanner.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aiursoft.Probe.Services
{
    public class FolderLocator : IScopedDependency
    {
        public string[] SplitToFolders(string folderNames) =>
            folderNames?.Split('/', StringSplitOptions.RemoveEmptyEntries) ?? new string[0];

        public (string[] folders, string fileName) SplitToFoldersAndFile(string folderNames)
        {
            if (folderNames == null || folderNames.Length == 0)
            {
                throw new AiurAPIModelException(ErrorType.NotFound, "The root folder isn't a file!");
            }
            var foldersWithFileName = SplitToFolders(folderNames);
            var fileName = foldersWithFileName.Last();
            var folders = foldersWithFileName.Take(foldersWithFileName.Count() - 1).ToArray();
            return (folders, fileName);
        }

        public string GetValidFileName(IEnumerable<string> existingFileNames, string expectedFileName)
        {
            while (existingFileNames.Any(t => t.ToLower() == expectedFileName.ToLower()))
            {
                expectedFileName = "_" + expectedFileName;
            }
            return expectedFileName;
        }
    }
}
