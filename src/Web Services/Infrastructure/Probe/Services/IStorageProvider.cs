using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Services
{
    public interface IStorageProvider
    {
        void Delete(int id);
        long GetSize(int id);
        Task Save(int id, IFormFile file);
        string GetFilePath(int id);
        string GetExtension(string fileName);
    }
}
