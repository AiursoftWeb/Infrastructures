using Aiursoft.Pylon.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Stargate.Data
{
    public class TimeoutCleaner
    {
        private StargateMemory _memoryContext;
        public TimeoutCleaner(StargateMemory memoryContext)
        {
            _memoryContext = memoryContext;
        }
        public async Task AllClean(StargateDbContext _dbContext)
        {
            try
            {
                _memoryContext.Messages.RemoveAll(t => t.CreateTime < DateTime.Now - new TimeSpan(0, 1, 0));
                _dbContext.Channels.RemoveRange(_dbContext.Channels.ToList().Where(t => !t.IsAlive()));
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {

            }
        }
    }
}
