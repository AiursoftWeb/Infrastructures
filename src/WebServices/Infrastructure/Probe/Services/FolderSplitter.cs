using System;
using System.Collections.Generic;
using System.Linq;
using Aiursoft.AiurProtocol.Exceptions;
using Aiursoft.AiurProtocol.Models;
using Aiursoft.Scanner.Abstract;

namespace Aiursoft.Probe.Services;

public class FolderSplitter : IScopedDependency
{
    private readonly string[] _invalidStrings = { "+" };

    public string[] SplitToFolders(string folderNames)
    {
        return folderNames?.Split('/', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
    }

    public (string[] folders, string fileName) SplitToFoldersAndFile(string folderNames)
    {
        if (string.IsNullOrEmpty(folderNames))
        {
            throw new AiurServerException(Code.NotFound, "The root folder isn't a file!");
        }

        var foldersWithFileName = SplitToFolders(folderNames);
        var fileName = foldersWithFileName.Last();
        var folders = foldersWithFileName.Take(foldersWithFileName.Length - 1).ToArray();
        return (folders, fileName);
    }

    public string GetValidFileName(IEnumerable<string> existingFileNames, string expectedFileName)
    {
        var fileNames = existingFileNames.ToArray();
        while (fileNames.Any(t => t.ToLower() == expectedFileName.ToLower()))
        {
            expectedFileName = "_" + expectedFileName;
        }

        expectedFileName = expectedFileName.ToLower();
        foreach (var invalidChar in _invalidStrings)
        {
            expectedFileName = expectedFileName.Replace(invalidChar, string.Empty);
        }

        return expectedFileName;
    }
}