using System;

namespace Aiursoft.Stargate.SDK.Models
{
    public class ChannelDetail
    {
        public DateTime LastAccessTime { get; set; }
        public Channel Channel { get; set; }

        public ChannelDetail(
            Channel channel,
            int connectedCount,
            DateTime lastAccessTime)
        {
            Channel = channel;
            ConnectedCount = connectedCount;
            LastAccessTime = lastAccessTime;
        }

        public int ConnectedCount { get; set; }
    }
}
