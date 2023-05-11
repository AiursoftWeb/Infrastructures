using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Aiursoft.Stargate.SDK.Models;

public class Channel
{
    public int Id { get; set; }
    public string Description { get; set; }
    public string ConnectKey { get; set; }
    public DateTime CreateTime { get; set; } = DateTime.UtcNow;
    public DateTime LastAccessTime { get; set; } = DateTime.UtcNow;
    public string AppId { get; set; }

    [JsonIgnore] public ConcurrentBag<Message> Messages { get; set; } = new();

    [JsonIgnore] public TimeSpan MaxIdleLife { get; } = TimeSpan.FromDays(10);

    public int ConnectedUsers { get; set; }

    public IEnumerable<Message> GetMessagesFrom(int lastReadId)
    {
        return Messages.Where(t => t.Id > lastReadId);
    }

    public void Push(Message message)
    {
        Messages.Add(message);
    }

    public bool IsAlive()
    {
        return !IsDead();
    }

    public bool IsDead()
    {
        return LastAccessTime + MaxIdleLife < DateTime.UtcNow;
    }
}