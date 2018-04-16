using System;
using System.Collections.Generic;
using System.Text;

namespace Aiursoft.Pylon.Models
{
    public enum EventType : int
    {
        NewMessage = 0,
        NewFriendRequest = 1,
        WereDeletedEvent = 2,
        FriendAcceptedEvent = 3
    }
    public abstract class AiurEvent
    {
        public EventType Type { get; set; }
    }
    public class NewMessageEvent : AiurEvent
    {
        public int ConversationId { get; set; }
        public AiurUserBase Sender { get; set; }
        public string Content { get; set; }
    }
    public class NewFriendRequest : AiurEvent
    {
        public string RequesterId { get; set; }
    }
    public class WereDeletedEvent : AiurEvent
    {

    }
    public class FriendAcceptedEvent : AiurEvent
    {

    }
}
