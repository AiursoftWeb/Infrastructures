using Aiursoft.Scanner.Interfaces;
using Aiursoft.Stargate.SDK.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Aiursoft.Stargate.Data
{
    public class StargateMemory : ISingletonDependency
    {
        public ConcurrentDictionary<int, ConcurrentBag<Message>> Channels = new ConcurrentDictionary<int, ConcurrentBag<Message>>();

        public void CreateChannel(int id)
        {
            Channels[id] = new ConcurrentBag<Message>();
        }

        public bool ChannelExists(int id)
        {
            return Channels.ContainsKey(id);
        }

        public IEnumerable<Message> GetMessages(int id, int lastReadId)
        {
            return Channels[id]
                .Where(t => t.Id > lastReadId);
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

        public void DeleteChannel(int channel)
        {
            Channels.Remove(channel, out ConcurrentBag<Message> deleted);
        }

        public int GetTotalMessages()
        {
            return Channels
                .SelectMany(t => t.Value)
                .Count();
        }

        public void Push(int id, Message message)
        {
            Channels[id].Add(message);
        }
    }
}
