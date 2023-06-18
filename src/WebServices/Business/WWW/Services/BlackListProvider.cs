using Aiursoft.Canon;
using Aiursoft.Scanner.Abstract;
using Aiursoft.XelNaga.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.XelNaga.Models;

namespace Aiursoft.WWW.Services;

public class BlackListProvider : IScopedDependency
{
    private readonly string _address;
    private readonly HttpService _httpService;
    private readonly CacheService _cache;
    private readonly RetryEngine _retryEngine;

    public BlackListProvider(
        HttpService httpService,
        
        // TODO: Use IOptions!
        IConfiguration configuration,
        CacheService cache,
        RetryEngine retryEngine)
    {
        _address = configuration["BlackListLocation"];
        _httpService = httpService;
        _cache = cache;
        _retryEngine = retryEngine;
    }

    private Task<string[]> GetItems()
    {
        return _cache.RunWithCache("black_list", () =>
        {
            return _retryEngine.RunWithRetry(async _ =>
            {
                var list = await _httpService.Get(new AiurUrl(_address));
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
        var blackListItem = await GetItems();
        return blackListItem.Any(t => domain.ToLower().Trim().EndsWith(t.ToLower().Trim()));
    }
}