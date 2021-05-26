using Aiursoft.Probe.Data;
using Aiursoft.Probe.SDK.Models;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.SDKTools.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Services
{
    public class FileDeleter : ITransientDependency
    {
        private readonly ProbeDbContext _probeDbContext;
        private readonly RetryEngine _retryEngine;
        private readonly IStorageProvider _storageProvider;

        public FileDeleter(
            ProbeDbContext probeDbContext,
            RetryEngine retryEngine,
            IStorageProvider storageProvider)
        {
            this._probeDbContext = probeDbContext;
            this._retryEngine = retryEngine;
            this._storageProvider = storageProvider;
        }

        public async Task DeleteOnDisk(File file)
        {
            var haveDaemon = await _probeDbContext.Files.Where(f => f.Id != file.Id).AnyAsync(f => f.HardwareId == file.HardwareId);
            if (!haveDaemon)
            {
                await this._retryEngine.RunWithTry(taskFactory: attempt =>
                {
                    _storageProvider.DeleteToTrash(file.HardwareId);
                    return Task.FromResult(0);
                }, attempts: 10);
            }
        }
    }
}
