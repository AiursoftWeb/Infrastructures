using System.Collections.Concurrent;
using Aiursoft.Scanner.Abstractions;
using Aiursoft.Stargate.SDK.Models;

namespace Aiursoft.Stargate.Data;

public class StargateMemory : ISingletonDependency
{
    private readonly ConcurrentDictionary<int, Channel> Channels = new();

    public Channel this[int id]
    {
        get
        {
            if (!ChannelExists(id))
            {
                return null;
            }

            return Channels[id];
        }
    }

    private bool ChannelExists(int id)
    {
        return Channels.ContainsKey(id);
    }

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

    public IEnumerable<Channel> GetChannelsUnderApp(string appid)
    {
        return Channels.Select(t => t.Value).Where(t => t.AppId == appid);
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
        Channels[id] = null;
        return Channels.Remove(id, out _);
    }
}