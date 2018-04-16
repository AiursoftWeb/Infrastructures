using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.ToStargateServer;
using Aiursoft.Pylon.Models.Stargate.ChannelViewModels;
using Kahla.Server.Data;
using Aiursoft.Pylon.Models;
using Newtonsoft.Json;
using Kahla.Server.Models;
using Newtonsoft.Json.Serialization;

namespace Kahla.Server.Services
{
    public class PushKahlaMessageService
    {
        private readonly KahlaDbContext _dbContext;
        private readonly PushMessageService _pushMessageService;
        public PushKahlaMessageService(
            KahlaDbContext dbContext,
            PushMessageService pushMessageService)
        {
            _dbContext = dbContext;
            _pushMessageService = pushMessageService;
        }
        private string _CammalSer(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

        public async Task<CreateChannelViewModel> Init()
        {
            var token = AppsContainer.AccessToken();
            var channel = await ChannelService.CreateChannelAsync(await token(), "");
            return channel;
        }

        public async Task NewMessageEvent(string recieverId, int conversationId, string Content, KahlaUser sender)
        {
            var token = AppsContainer.AccessToken();
            var user = await _dbContext.Users.FindAsync(recieverId);
            var channel = user.CurrentChannel;
            var nevent = new NewMessageEvent
            {
                Type = EventType.NewMessage,
                ConversationId = conversationId,
                Sender = sender,
                Content = Content
            };
            if (channel != -1)
                await _pushMessageService.PushMessageAsync(await token(), channel, _CammalSer(nevent), true);
        }

        public async Task NewFriendRequestEvent(string recieverId, string requesterId)
        {
            var token = AppsContainer.AccessToken();
            var user = await _dbContext.Users.FindAsync(recieverId);
            var channel = user.CurrentChannel;
            var nevent = new NewFriendRequest
            {
                Type = EventType.NewFriendRequest,
                RequesterId = requesterId
            };
            if (channel != -1)
                await _pushMessageService.PushMessageAsync(await token(), channel, _CammalSer(nevent), true);
        }

        public async Task WereDeletedEvent(string recieverId)
        {
            var token = AppsContainer.AccessToken();
            var user = await _dbContext.Users.FindAsync(recieverId);
            var channel = user.CurrentChannel;
            var nevent = new WereDeletedEvent
            {
                Type = EventType.WereDeletedEvent
            };
            if (channel != -1)
                await _pushMessageService.PushMessageAsync(await token(), channel, _CammalSer(nevent), true);
        }

        public async Task FriendAcceptedEvent(string recieverId)
        {
            var token = AppsContainer.AccessToken();
            var user = await _dbContext.Users.FindAsync(recieverId);
            var channel = user.CurrentChannel;
            var nevent = new FriendAcceptedEvent
            {
                Type = EventType.FriendAcceptedEvent
            };
            if (channel != -1)
                await _pushMessageService.PushMessageAsync(await token(), channel, _CammalSer(nevent), true);
        }
    }
}
