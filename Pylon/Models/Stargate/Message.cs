using System;

namespace Aiursoft.Pylon.Models.Stargate
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;

        public int ChannelId { get; set; }
    }
}
