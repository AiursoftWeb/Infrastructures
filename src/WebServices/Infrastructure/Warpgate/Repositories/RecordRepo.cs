using Aiursoft.AiurProtocol.Exceptions;
using Aiursoft.AiurProtocol.Models;
using Aiursoft.DbTools;
using Aiursoft.Scanner.Abstractions;
using Aiursoft.Warpgate.Data;
using Aiursoft.Warpgate.SDK.Models;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Warpgate.Repositories;

public class RecordRepo : IScopedDependency
{
    private static readonly SemaphoreSlim CreateRecordLock = new(1, 1);
    private readonly WarpgateDbContext _dbContext;
    private readonly DbSet<WarpRecord> _table;

    public RecordRepo(WarpgateDbContext dbContext)
    {
        _dbContext = dbContext;
        _table = dbContext.Records;
    }

    public Task DeleteRecord(WarpRecord record)
    {
        _table.Remove(record);
        return _dbContext.SaveChangesAsync();
    }

    public Task<List<WarpRecord>> GetAllRecords(string appId)
    {
        return _table.Where(t => t.AppId == appId).ToListAsync();
    }

    public Task<WarpRecord> GetRecordByName(string recordName)
    {
        return _table.SingleOrDefaultAsync(t => t.RecordUniqueName == recordName.ToLower());
    }

    public async Task<WarpRecord> CreateRecord(string newRecordName, RecordType type, string appid, string targetUrl,
        bool enabled, string tags)
    {
        await CreateRecordLock.WaitAsync();
        try
        {
            var conflict = await _table.EnsureUniqueString(t => t.RecordUniqueName, newRecordName);
            if (conflict)
            {
                throw new AiurServerException(Code.Conflict, $"There is already a record with name: '{newRecordName}'. Please try another new name.");
            }
            var newRecord = new WarpRecord
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
        finally
        {
            CreateRecordLock.Release();
        }
    }

    public async Task<List<WarpRecord>> GetAllRecordsUnderApp(string appid, string mustHaveTags)
    {
        var query = _table.Where(t => t.AppId == appid);
        if (string.IsNullOrWhiteSpace(mustHaveTags))
        {
            return await query.ToListAsync();
        }

        var loadInMemoryResults = await query.ToListAsync();
        return loadInMemoryResults
            .Where(t => t.Tags?.Split(",").Any(s => s == mustHaveTags) ?? false)
            .ToList();
    }

    public async Task<WarpRecord> GetRecordByNameUnderApp(string recordName, string appid)
    {
        var record = await GetRecordByName(recordName);
        if (record == null)
        {
            throw new AiurServerException(Code.NotFound, $"Could not find a record with name: '{recordName}'");
        }

        if (record.AppId != appid)
        {
            throw new AiurServerException(Code.Unauthorized,
                "The record you tried to access is not your app's record.");
        }

        return record;
    }

    public async Task UpdateRecord(WarpRecord record)
    {
        _table.Update(record);
        await _dbContext.SaveChangesAsync();
    }
}