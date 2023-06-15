using Aiursoft.Canon;
using Aiursoft.Scanner.Abstract;
using Aiursoft.XelNaga.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.WWW.Services;

public class BlackListPorivder : IScopedDependency
{
    private readonly string _address;
    private readonly CacheService _cache;
    private readonly RetryEngine _retryEngine;

    public BlackListPorivder(

        // TODO: Use IOptions!
        IConfiguration configuration,
        CacheService cache,
        RetryEngine retryEngine)
    {
        this._address = configuration["BlackListLocation"];
        this._cache = cache;
        this._retryEngine = retryEngine;
    }

    private Task<string[]> GetItems()
    {
        return this._cache.RunWithCache("black_list", () =>
        {
            return _retryEngine.RunWithRetry(async attempt =>
            {
                var list = await SimpleHttp.DownloadAsString(_address);
                return list
                    .Split('\n')
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .ToArray();
            });
        });
    }

    public async Task<bool> InBlackListAsync(string input)
    {
        var url = new Uri(input);
        var domain = url.Host;
        var blackListItem = await this.GetItems();
        return blackListItem.Any(t => domain.ToLower().Trim().EndsWith(t.ToLower().Trim()));
    }
}