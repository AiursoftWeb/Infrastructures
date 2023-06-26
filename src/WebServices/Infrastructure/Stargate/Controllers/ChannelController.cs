using System;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.AiurProtocol;
using Aiursoft.AiurProtocol.Server;
using Aiursoft.Directory.SDK.Services;
using Aiursoft.Stargate.Data;
using Aiursoft.Stargate.SDK.Models;
using Aiursoft.Stargate.SDK.Models.ChannelAddressModels;
using Aiursoft.Stargate.SDK.Models.ChannelViewModels;
using Aiursoft.Stargate.SDK.Models.ListenAddressModels;
using Aiursoft.CSTools.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Stargate.Controllers;

[ApiExceptionHandler]
[ApiModelStateChecker]
public class ChannelController : ControllerBase
{
    private readonly Counter _counter;
    private readonly StargateDbContext _dbContext;
    private readonly StargateMemory _stargateMemory;
    private readonly AiursoftAppTokenValidator _tokenManager;

    public ChannelController(
        StargateDbContext dbContext,
        AiursoftAppTokenValidator tokenManager,
        StargateMemory stargateMemory,
        Counter counter)
    {
        _dbContext = dbContext;
        _tokenManager = tokenManager;
        _stargateMemory = stargateMemory;
        _counter = counter;
    }

    [Produces(typeof(ViewMyChannelsViewModel))]
    public async Task<IActionResult> ViewMyChannels(ViewMyChannelsAddressModel model)
    {
        var appid = await _tokenManager.ValidateAccessTokenAsync(model.AccessToken);
        var appLocal = await _dbContext.Apps.SingleOrDefaultAsync(t => t.Id == appid);
        if (appLocal == null)
        {
            appLocal = new StargateApp
            {
                Id = appid
            };
            await _dbContext.Apps.AddAsync(appLocal);
            await _dbContext.SaveChangesAsync();
        }

        var channels = _stargateMemory
            .GetChannelsUnderApp(appid);
        var viewModel = new ViewMyChannelsViewModel
        {
            AppId = appid,
            Channels = channels,
            Code = Code.ResultShown,
            Message = "Successfully get your channels!"
        };
        return this.Protocol(viewModel);
    }

    [Produces(typeof(AiurValue<string>))]
    public IActionResult ValidateChannel(ChannelAddressModel model)
    {
        var channel = _stargateMemory[model.Id];
        if (channel == null)
        {
            return this.Protocol(new AiurResponse
            {
                Code = Code.NotFound,
                Message = "Can not find your channel!"
            });
        }

        if (channel.IsDead())
        {
            return this.Protocol(new AiurResponse
            {
                Code = Code.Timeout,
                Message = "Your channel is out dated and about to be deleted!"
            });
        }

        if (channel.ConnectKey != model.Key)
        {
            return this.Protocol(new AiurResponse
            {
                Code = Code.Unauthorized,
                Message = "Wrong connection key!"
            });
        }

        return this.Protocol(new AiurValue<string>(channel.AppId)
        {
            Code = Code.ResultShown,
            Message = $"Current Info. Belongs to app with id: '{channel.AppId}'."
        });
    }

    [HttpPost]
    [Produces(typeof(CreateChannelViewModel))]
    public async Task<IActionResult> CreateChannel(CreateChannelAddressModel model)
    {
        //Update app info
        var appid = await _tokenManager.ValidateAccessTokenAsync(model.AccessToken);
        var appLocal = await _dbContext.Apps.SingleOrDefaultAsync(t => t.Id == appid);
        if (appLocal == null)
        {
            appLocal = new StargateApp
            {
                Id = appid
            };
            await _dbContext.Apps.AddAsync(appLocal);
        }

        var channelId = _counter.GetUniqueNo();
        var key = Guid.NewGuid().ToString("N");
        _stargateMemory.CreateChannel(channelId, appid, model.Description, key);
        //return model
        var viewModel = new CreateChannelViewModel
        {
            ChannelId = channelId,
            ConnectKey = key,
            Code = Code.JobDone,
            Message = "Successfully created your channel!"
        };
        return this.Protocol(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteChannel(DeleteChannelAddressModel model)
    {
        var appid =await _tokenManager.ValidateAccessTokenAsync(model.AccessToken);
        var channel = _stargateMemory[model.ChannelId];
        if (channel.AppId != appid)
        {
            return this.Protocol(new AiurResponse
            {
                Code = Code.Unauthorized, Message = "The channel you try to delete is not your app's channel!"
            });
        }

        _stargateMemory.DeleteChannel(channel.Id);
        await _dbContext.SaveChangesAsync();
        return this.Protocol(new AiurResponse
            { Code = Code.JobDone, Message = "Successfully deleted your channel!" });
    }

    /// <summary>
    ///     This action will delete all channels he created!
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> DeleteApp(DeleteAppAddressModel model)
    {
        var appid =await _tokenManager.ValidateAccessTokenAsync(model.AccessToken);
        if (appid != model.AppId)
        {
            return this.Protocol(new AiurResponse
            {
                Code = Code.Unauthorized, Message = "The app you try to delete is not the access token you granted!"
            });
        }

        var target = await _dbContext.Apps.FindAsync(appid);
        if (target == null)
        {
            
            // No Action needed.
            return this.Protocol(new AiurResponse
                { Code = Code.NotFound, Message = "That app do not exists in our database." });
        }

        _stargateMemory.DeleteChannels(_stargateMemory.GetChannelsUnderApp(appid).Select(t => t.Id));
        _dbContext.Apps.Remove(target);
        await _dbContext.SaveChangesAsync();
        return this.Protocol(new AiurResponse
            { Code = Code.JobDone, Message = "Successfully deleted that app and all channels." });
    }
}