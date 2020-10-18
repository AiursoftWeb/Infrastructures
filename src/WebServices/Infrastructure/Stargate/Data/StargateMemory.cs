using Aiursoft.Scanner.Interfaces;
using Aiursoft.Stargate.SDK.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Aiursoft.Stargate.Data
{
    public class StargateMemory : ISingletonDependency
    {
        private ConcurrentDictionary<int, Channel> Channels = new ConcurrentDictionary<int, Channel>();

        public void CreateChannel(int id, string appid, string description, string key)
        {
            Channels[id] = new Channel
            {
                Id = id,
                AppId = appid,
                Description = description,
                ConnectKey = key
            };
        }

        private bool ChannelExists(int id)
        {
            return Channels.ContainsKey(id);
        }

        public IEnumerable<Channel> GetChannelsUnderApp(string appid)
        {
            return Channels.Select(t => t.Value).Where(t => t.AppId == appid);
        }

        public IEnumerable<Channel> GetDeadChannels()
        {
            return Channels.Select(t => t.Value).Where(t => t.IsDead());
        }

        public Channel GetChannelById(int id)
        {
            if (!ChannelExists(id))
            {
                return null;
            }
            return Channels[id];
        }

        public void DeleteChannels(IEnumerable<int> channels)
        {
            foreach (var channel in channels)
            {
                if (ChannelExists(channel))
                {
                    DeleteChannel(channel);
                }
            }
        }

        public bool DeleteChannel(int id)
        {
            return Channels.Remove(id, out _);
        }

        public (int channelsCount, int totalMessages) GetMonitoringReport()
        {
            return (Channels.Count, Channels
                .SelectMany(t => t.Value.Messages)
                .Count());
        }
    }
}
