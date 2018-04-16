using Aiursoft.Pylon.Models.Stargate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Stargate.Data
{
    public static class StargateMemory
    {
        public static List<Message> Messages { get; set; } = new List<Message>();
    }
}
