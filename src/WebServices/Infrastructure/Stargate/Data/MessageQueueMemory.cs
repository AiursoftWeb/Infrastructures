using Aiursoft.Scanner.Interfaces;
using Aiursoft.Stargate.SDK.Models;
using System.Collections.Concurrent;

namespace Aiursoft.Stargate.Data
{
    public class StargateMemory : ISingletonDependency
    {
        public ConcurrentBag<Message> Messages { get; set; } = new ConcurrentBag<Message>();
    }
}
