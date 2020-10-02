using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
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
        private readonly DbSet<WrapRecord> _table;

        public RecordRepo(WrapgateDbContext dbContext)
        {
            _dbContext = dbContext;
            _table = dbContext.Records;
        }

        public Task DeleteRecord(WrapRecord record)
        {
            _table.Remove(record);
            return _dbContext.SaveChangesAsync();
        }

        public Task<List<WrapRecord>> GetAllRecords(string appId)
        {
            return _table.Where(t => t.AppId == appId).ToListAsync();
        }

        public Task<WrapRecord> GetRecordByName(string recordName)
        {
            return _table.SingleOrDefaultAsync(t => t.RecordUniqueName == recordName.ToLower());
        }

        public async Task<WrapRecord> CreateRecord(string newRecordName, RecordType type, string appid, string targetUrl, bool enabled, string tags)
        {
            var newRecord = new WrapRecord
            {
                RecordUniqueName = newRecordName.ToLower(),
                Type = type,
                AppId = appid,
                TargetUrl = targetUrl,
                Enabled = enabled,
                Tags = tags
            };
            await _table.AddAsync(newRecord);
            await _dbContext.SaveChangesAsync();
            return newRecord;
        }

        public Task<List<WrapRecord>> GetAllRecordsUnderApp(string appid, string[] mustHaveTags)
        {
            if (!mustHaveTags.Any())
            {
                return _table
                    .Where(t => t.AppId == appid)
                    .ToListAsync();
            }
            else
            {
                return _table
                    .Where(t => t.AppId == appid)
                    .Where(t => mustHaveTags.Any(p => t.Tags.Contains(p)))
                    .ToListAsync();
            }
        }

        public async Task<WrapRecord> GetRecordByNameUnderApp(string recordName, string appid)
        {
            var record = await GetRecordByName(recordName);
            if (record == null)
            {
                throw new AiurAPIModelException(ErrorType.NotFound, $"Could not find a record with name: '{recordName}'");
            }
            if (record.AppId != appid)
            {
                throw new AiurAPIModelException(ErrorType.Unauthorized, "The site you tried to access is not your app's site.");
            }
            return record;
        }

        public async Task UpdateRecord(WrapRecord record)
        {
            _table.Update(record);
            await _dbContext.SaveChangesAsync();
        }
    }
}
