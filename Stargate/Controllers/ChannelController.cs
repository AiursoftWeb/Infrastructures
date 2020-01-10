using Aiursoft.Pylon.Attributes;
using Aiursoft.SDK.Models.Stargate;
using Aiursoft.SDK.Models.Stargate.ChannelAddressModels;
using Aiursoft.SDK.Models.Stargate.ChannelViewModels;
using Aiursoft.SDK.Models.Stargate.ListenAddressModels;
using Aiursoft.Stargate.Data;
using Aiursoft.Stargate.Services;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Aiursoft.XelNaga.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Stargate.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    public class ChannelController : Controller
    {
        private readonly StargateDbContext _dbContext;
        private readonly ACTokenManager _tokenManager;
        private readonly StargateMemory _stargateMemory;
        private readonly ConnectedCountService _connectedCountService;
        private readonly LastAccessService _lastAccessService;

        public ChannelController(
            StargateDbContext dbContext,
            ACTokenManager tokenManager,
            StargateMemory stargateMemory,
            ConnectedCountService connectedCountService,
            LastAccessService lastAccessService)
        {
            _dbContext = dbContext;
            _tokenManager = tokenManager;
            _stargateMemory = stargateMemory;
            _connectedCountService = connectedCountService;
            _lastAccessService = lastAccessService;
        }

        [APIProduces(typeof(ViewMyChannelsViewModel))]
        public async Task<IActionResult> ViewMyChannels(ViewMyChannelsAddressModel model)
        {
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
            var appLocal = await _dbContext.Apps.SingleOrDefaultAsync(t => t.Id == appid);
            if (appLocal == null)
            {
                appLocal = new StargateApp
                {
                    Id = appid,
                    Channels = new List<Channel>()
                };
                _dbContext.Apps.Add(appLocal);
                await _dbContext.SaveChangesAsync();
            }
            var channels = await _dbContext
                .Channels
                .Where(t => t.AppId == appid)
                .ToListAsync();
            var viewModel = new ViewMyChannelsViewModel
            {
                AppId = appLocal.Id,
                Channel = channels
                    .Select(t => new ChannelDetail(t,
                    _connectedCountService.GetConnectedCount(t.Id),
                    _lastAccessService.GetLastAccessTime(t.Id)))
                    .ToList(),
                Code = ErrorType.Success,
                Message = "Successfully get your channels!"
            };
            return Json(viewModel);
        }

        [APIProduces(typeof(AiurValue<string>))]
        public async Task<IActionResult> ValidateChannel(ChannelAddressModel model)
        {
            var channel = await _dbContext.Channels.FindAsync(model.Id);
            if (channel == null)
            {
                return Json(new AiurProtocol
                {
                    Code = ErrorType.NotFound,
                    Message = "Can not find your channel!"
                });
            }
            if (!channel.IsAlive)
            {
                return Json(new AiurProtocol
                {
                    Code = ErrorType.Pending,
                    Message = "Your channel is out dated and about to be deleted!"
                });
            }
            if (channel.ConnectKey != model.Key)
            {
                return Json(new AiurProtocol
                {
                    Code = ErrorType.Unauthorized,
                    Message = "Wrong connection key!"
                });
            }
            else
            {
                return Json(new AiurValue<string>(channel.AppId)
                {
                    Code = ErrorType.Success,
                    Message = $"Current Info. Belongs to app with id: '{channel.AppId}'."
                });
            }
        }

        [HttpPost]
        [APIProduces(typeof(CreateChannelViewModel))]
        public async Task<IActionResult> CreateChannel(CreateChannelAddressModel model)
        {
            //Update app info
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
            var appLocal = await _dbContext.Apps.Include(t => t.Channels).SingleOrDefaultAsync(t => t.Id == appid);
            if (appLocal == null)
            {
                appLocal = new StargateApp
                {
                    Id = appid,
                    Channels = new List<Channel>()
                };
                _dbContext.Apps.Add(appLocal);
            }
            //Create and save to database
            var newChannel = new Channel
            {
                Description = model.Description,
                ConnectKey = Guid.NewGuid().ToString("N")
            };
            appLocal.Channels.Add(newChannel);
            await _dbContext.SaveChangesAsync();
            //return model
            var viewModel = new CreateChannelViewModel
            {
                ChannelId = newChannel.Id,
                ConnectKey = newChannel.ConnectKey,
                Code = ErrorType.Success,
                Message = "Successfully created your channel!"
            };
            return Json(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteChannel(DeleteChannelAddressModel model)
        {
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
            var channel = await _dbContext.Channels.FindAsync(model);
            if (channel.AppId != appid)
            {
                return Json(new AiurProtocol { Code = ErrorType.Unauthorized, Message = "The channel you try to delete is not your app's channel!" });
            }
            _dbContext.Channels.Remove(channel);
            await _dbContext.SaveChangesAsync();
            return Json(new AiurProtocol { Code = ErrorType.Success, Message = "Successfully deleted your channel!" });
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
                return Json(new AiurProtocol { Code = ErrorType.Unauthorized, Message = "The app you try to delete is not the accesstoken you granted!" });
            }
            var target = await _dbContext.Apps.FindAsync(appid);
            if (target != null)
            {
                _dbContext.Channels.Delete(t => t.AppId == target.Id);
                _dbContext.Apps.Remove(target);
                await _dbContext.SaveChangesAsync();
                return Json(new AiurProtocol { Code = ErrorType.Success, Message = "Successfully deleted that app and all channels." });
            }
            return Json(new AiurProtocol { Code = ErrorType.HasDoneAlready, Message = "That app do not exists in our database." });
        }
    }
}