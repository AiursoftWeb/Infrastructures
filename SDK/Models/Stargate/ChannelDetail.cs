namespace Aiursoft.SDK.Models.Stargate
{
    public class ChannelDetail
    {
        public Channel Channel { get; set; }

        public ChannelDetail(Channel channel, int connectedCount)
        {
            Channel = channel;
            ConnectedCount = connectedCount;
        }

        public int ConnectedCount { get; set; }
    }
}
