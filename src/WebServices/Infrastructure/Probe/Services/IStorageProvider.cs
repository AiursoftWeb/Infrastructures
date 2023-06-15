using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Aiursoft.Probe.Services;

public interface IStorageProvider
{
    void Delete(string hardwareUuid);
    void DeleteToTrash(string hardwareUuid);
    Task<long> GetSize(string hardwareUuid);
    Task Save(string hardwareUuid, IFormFile file);
    string GetFilePath(string hardwareUuid);
    string GetExtension(string fileName);
    bool ExistInHardware(string hardwareUuid);
    string[] GetAllFileNamesInHardware();
}