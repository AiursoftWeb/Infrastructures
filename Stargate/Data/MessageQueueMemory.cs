using Aiursoft.Scanner.Interfaces;
using Aiursoft.Stargate.SDK.Models;
using System.Collections.Generic;

namespace Aiursoft.Stargate.Data
{
    public class StargateMemory : ISingletonDependency
    {
        public List<Message> Messages { get; set; } = new List<Message>();
    }
}
