using Aiursoft.Archon.SDK.Services;
using Aiursoft.DocGenerator.Attributes;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.Stargate.Data;
using Aiursoft.Stargate.SDK.Models;
using Aiursoft.Stargate.SDK.Models.ChannelAddressModels;
using Aiursoft.Stargate.SDK.Models.ChannelViewModels;
using Aiursoft.Stargate.SDK.Models.ListenAddressModels;
using Aiursoft.WebTools;
using Aiursoft.XelNaga.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Stargate.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    public class ChannelController : ControllerBase
    {
        private readonly StargateDbContext _dbContext;
        private readonly ACTokenValidator _tokenManager;
        private readonly StargateMemory _stargateMemory;
        private readonly Counter _counter;

        public ChannelController(
            StargateDbContext dbContext,
            ACTokenValidator tokenManager,
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
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
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
                Code = ErrorType.Success,
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
                return this.Protocol(new AiurProtocol
                {
                    Code = ErrorType.NotFound,
                    Message = "Can not find your channel!"
                });
            }
            if (channel.IsDead())
            {
                return this.Protocol(new AiurProtocol
                {
                    Code = ErrorType.Gone,
                    Message = "Your channel is out dated and about to be deleted!"
                });
            }
            if (channel.ConnectKey != model.Key)
            {
                return this.Protocol(new AiurProtocol
                {
                    Code = ErrorType.Unauthorized,
                    Message = "Wrong connection key!"
                });
            }
            else
            {
                return this.Protocol(new AiurValue<string>(channel.AppId)
                {
                    Code = ErrorType.Success,
                    Message = $"Current Info. Belongs to app with id: '{channel.AppId}'."
                });
            }
        }

        [HttpPost]
        [Produces(typeof(CreateChannelViewModel))]
        public async Task<IActionResult> CreateChannel(CreateChannelAddressModel model)
        {
            //Update app info
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
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
                Code = ErrorType.Success,
                Message = "Successfully created your channel!"
            };
            return this.Protocol(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteChannel(DeleteChannelAddressModel model)
        {
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
            var channel = _stargateMemory[model.ChannelId];
            if (channel.AppId != appid)
            {
                return this.Protocol(new AiurProtocol { Code = ErrorType.Unauthorized, Message = "The channel you try to delete is not your app's channel!" });
            }
            _stargateMemory.DeleteChannel(channel.Id);
            await _dbContext.SaveChangesAsync();
            return this.Protocol(new AiurProtocol { Code = ErrorType.Success, Message = "Successfully deleted your channel!" });
        }

        /// <summary>
        /// This action will delete all channels he created!
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteApp(DeleteAppAddressModel model)
        {
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
            if (appid != model.AppId)
            {
                return this.Protocol(new AiurProtocol { Code = ErrorType.Unauthorized, Message = "The app you try to delete is not the accesstoken you granted!" });
            }
            var target = await _dbContext.Apps.FindAsync(appid);
            if (target == null)
                return this.Protocol(new AiurProtocol
                    { Code = ErrorType.HasSuccessAlready, Message = "That app do not exists in our database." });
            _stargateMemory.DeleteChannels(_stargateMemory.GetChannelsUnderApp(appid).Select(t => t.Id));
            _dbContext.Apps.Remove(target);
            await _dbContext.SaveChangesAsync();
            return this.Protocol(new AiurProtocol { Code = ErrorType.Success, Message = "Successfully deleted that app and all channels." });
        }
    }
}