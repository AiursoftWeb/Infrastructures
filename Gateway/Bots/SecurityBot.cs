using Kahla.SDK.Abstract;
using Kahla.SDK.Events;
using Kahla.SDK.Models.ApiViewModels;
using System.Threading.Tasks;

namespace Aiursoft.Gateway.Bots
{
    public class SecurityBot : BotBase
    {
        public override async Task OnBotInit()
        {
        }

        public override Task OnFriendRequest(NewFriendRequestEvent arg)
        {
            return CompleteRequest(arg.RequestId, true);
        }

        public override async Task OnGroupConnected(SearchedGroup group)
        {
        }

        public override async Task OnGroupInvitation(int groupId, NewMessageEvent eventContext)
        {
        }

        public override async Task OnMessage(string inputMessage, NewMessageEvent eventContext)
        {
        }
    }
}
