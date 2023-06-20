using System.Linq;
using System.Threading.Tasks;

using Aiursoft.AiurProtocol.Models;
using Aiursoft.Scanner.Abstract;
using Aiursoft.Warpgate.SDK.Configuration;
using Aiursoft.Warpgate.SDK.Models;
using Aiursoft.Warpgate.SDK.Models.AddressModels;
using Aiursoft.Warpgate.SDK.Models.ViewModels;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Aiursoft.Warpgate.SDK.Services.ToWarpgateServer;

public class RecordsService : IScopedDependency
{
    private readonly AiurProtocolClient  _http;
    private readonly WarpgateConfiguration _serviceLocation;

    public RecordsService(
        AiurProtocolClient  http,
        IOptions<WarpgateConfiguration> serviceLocation)
    {
        _http = http;
        _serviceLocation = serviceLocation.Value;
    }

    public async Task<AiurResponse> CreateNewRecordAsync(
        string accessToken,
        string newRecordName,
        string targetUrl,
        string[] tags,
        RecordType type,
        bool enabled)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance, "Records", "CreateNewRecord", new { });
        var form = new ApiPayload( new CreateNewRecordAddressModel
        {
            AccessToken = accessToken,
            NewRecordName = newRecordName,
            TargetUrl = targetUrl,
            Type = type,
            Enabled = enabled,
            Tags = string.Join(',', tags.Select(t => t.Trim()))
        });
        return await _http.Post<aaaaaaaa>(url, form);
        var jResult = JsonConvert.DeserializeObject<AiurResponse>(result);
        if (jResult.Code != ErrorType.Success)
        {
            throw new AiurUnexpectedResponse(jResult);
        }

        return jResult;
    }

    public async Task<ViewMyRecordsViewModel> ViewMyRecordsAsync(string accessToken, string tag = null)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance, "Records", "ViewMyRecords", new ViewMyRecordsAddressModel
        {
            AccessToken = accessToken,
            Tag = tag
        });
        return await _http.Get<bbbbbbbbbbb>(url);
        var jResult = JsonConvert.DeserializeObject<ViewMyRecordsViewModel>(result);
        if (jResult.Code != ErrorType.Success)
        {
            throw new AiurUnexpectedResponse(jResult);
        }

        return jResult;
    }

    public async Task<AiurResponse> UpdateRecordInfoAsync(
        string accessToken,
        string oldRecordName,
        string newRecordName,
        RecordType newType,
        string newUrl,
        string[] tags,
        bool enabled)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance, "Records", "UpdateRecordInfo", new { });
        var form = new ApiPayload( new UpdateRecordInfoAddressModel
        {
            AccessToken = accessToken,
            OldRecordName = oldRecordName,
            NewRecordName = newRecordName,
            NewType = newType,
            NewUrl = newUrl,
            Enabled = enabled,
            Tags = string.Join(',', tags.Select(t => t.Trim()))
        });
        return await _http.Post<aaaaaaaa>(url, form);
        var jResult = JsonConvert.DeserializeObject<AiurResponse>(result);
        if (jResult.Code != ErrorType.Success)
        {
            throw new AiurUnexpectedResponse(jResult);
        }

        return jResult;
    }

    public async Task<AiurResponse> DeleteRecordAsync(string accessToken, string recordName)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance, "Records", "DeleteRecord", new { });
        var form = new ApiPayload( new DeleteRecordAddressModel
        {
            AccessToken = accessToken,
            RecordName = recordName
        });
        return await _http.Post<aaaaaaaa>(url, form);
        var jResult = JsonConvert.DeserializeObject<AiurResponse>(result);
        if (jResult.Code != ErrorType.Success)
        {
            throw new AiurUnexpectedResponse(jResult);
        }

        return jResult;
    }

    public async Task<AiurResponse> DeleteAppAsync(string accessToken, string appId)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance, "Records", "DeleteApp", new { });
        var form = new ApiPayload( new DeleteAppAddressModel
        {
            AccessToken = accessToken,
            AppId = appId
        });
        return await _http.Post<aaaaaaaa>(url, form);
        var jResult = JsonConvert.DeserializeObject<AiurResponse>(result);
        if (jResult.Code != ErrorType.Success)
        {
            throw new AiurUnexpectedResponse(jResult);
        }

        return jResult;
    }
}