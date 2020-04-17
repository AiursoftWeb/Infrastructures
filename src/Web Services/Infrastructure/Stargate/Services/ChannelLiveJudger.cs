using Aiursoft.Scanner.Interfaces;
using System;

namespace Aiursoft.Stargate.Services
{
    public class ChannelLiveJudger : ISingletonDependency
    {
        private readonly ConnectedCountService _connectedCountService;
        private readonly LastAccessService _lastAccessService;
        private readonly TimeSpan _maxIdleLife = TimeSpan.FromDays(10);

        public ChannelLiveJudger(
            ConnectedCountService connectedCountService,
            LastAccessService lastAccessService)
        {
            _connectedCountService = connectedCountService;
            _lastAccessService = lastAccessService;
        }

        public bool IsAlive(int channelId)
        {
            return !IsDead(channelId);
        }

        public bool IsDead(int channelId)
        {
            var connected = _connectedCountService.GetConnectedCount(channelId);
            var lastAccess = _lastAccessService.GetLastAccessTime(channelId);
            return connected < 1 && lastAccess + _maxIdleLife < DateTime.UtcNow;
        }
    }
}
