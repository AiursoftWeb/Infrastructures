using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Services
{
    public interface IStorageProvider
    {
        void Delete(string hardwareUuid);
        long GetSize(string hardwareUuid);
        Task Save(string hardwareUuid, IFormFile file);
        string GetFilePath(string hardwareUuid);
        string GetExtension(string fileName);
    }
}
