using Aiursoft.Archon.SDK.Services;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.Stargate.Data;
using Aiursoft.Stargate.SDK.Models;
using Aiursoft.Stargate.SDK.Models.MessageAddressModels;
using Aiursoft.WebTools;
using Aiursoft.XelNaga.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Aiursoft.Stargate.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    public class MessageController : ControllerBase
    {
        private readonly StargateDbContext _dbContext;
        private readonly StargateMemory _memoryContext;
        private readonly Counter _counter;
        private readonly ACTokenValidator _tokenManager;

        public MessageController(
            StargateDbContext dbContext,
            StargateMemory memoryContext,
            Counter counter,
            ACTokenValidator tokenManager)
        {
            _dbContext = dbContext;
            _memoryContext = memoryContext;
            _counter = counter;
            _tokenManager = tokenManager;
        }

        [HttpPost]
        public IActionResult PushMessage(PushMessageAddressModel model)
        {
            //Ensure app
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
            //Ensure channel
            var channel = _memoryContext.GetChannelsUnderApp(appid).SingleOrDefault(t => t.Id == model.ChannelId);
            if (channel == null)
            {
                return this.Protocol(ErrorType.NotFound, $"We can not find your channel with id: '{model.ChannelId}'!");
            }
            //Create Message
            var message = new Message
            {
                Id = _counter.GetUniqueNo(),
                ChannelId = channel.Id,
                Content = model.MessageContent
            };
            channel.Push(message);
            return Ok(new AiurProtocol
            {
                Code = ErrorType.Success,
                Message = $"You have successfully pushed a new message to channel: {channel.Id}!"
            });
        }
    }
}
