using Aiursoft.Scanner.Interfaces;
using Aiursoft.Wrapgate.Data;
using Aiursoft.Wrapgate.SDK.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Wrapgate.Repositories
{
    public class RecordRepo : IScopedDependency
    {
        private readonly WrapgateDbContext _dbContext;

        public RecordRepo(WrapgateDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task DeleteRecord(WrapRecord record)
        {
            _dbContext.Records.Remove(record);
            return _dbContext.SaveChangesAsync();
        }

        public Task<List<WrapRecord>> GetAllRecords(string appId)
        {
            return _dbContext.Records.Where(t => t.AppId == appId).ToListAsync();
        }
    }
}
