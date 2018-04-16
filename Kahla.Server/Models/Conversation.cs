using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Kahla.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace Kahla.Server.Models
{
    public abstract class Conversation
    {
        [Key]
        public int Id { get; set; }

        public string Discriminator { get; set; }
        [JsonIgnore]
        [InverseProperty(nameof(Message.Conversation))]
        public IEnumerable<Message> Messages { get; set; }

        public DateTime ConversationCreateTime { get; set; } = DateTime.Now;

        [NotMapped]
        public string DisplayName { get; set; }
        [NotMapped]
        public string DisplayImage { get; set; }


        public abstract string GetDisplayName(string userId);
        public abstract string GetDisplayImage(string userId);
        public abstract int GetUnReadAmount(string userId);
        public abstract Message GetLatestMessage();
    }
    public class PrivateConversation : Conversation
    {
        public string RequesterId { get; set; }
        [ForeignKey(nameof(RequesterId))]
        public KahlaUser RequestUser { get; set; }

        public string TargetId { get; set; }
        [ForeignKey(nameof(TargetId))]
        public KahlaUser TargetUser { get; set; }
        [NotMapped]
        public string AnotherUserId { get; set; }

        public KahlaUser AnotherUser(string myId) => myId == RequesterId ? TargetUser : RequestUser;

        public override string GetDisplayImage(string userId) => this.AnotherUser(userId).HeadImgUrl;

        public override string GetDisplayName(string userId) => this.AnotherUser(userId).NickName;
        public override int GetUnReadAmount(string userId) => this.Messages.Where(p => !p.Read && p.SenderId != userId).Count();
        public override Message GetLatestMessage()
        {
            try
            {
                return this.Messages.OrderByDescending(p => p.SendTime).First();
            }
            catch (InvalidOperationException)
            {
                return new Message
                {
                    Content = "You are friends. Start chatting now!",
                    SendTime = this.ConversationCreateTime
                };
            }
        }

    }

    public class GroupConversation : Conversation
    {
        [JsonIgnore]
        [InverseProperty(nameof(UserGroupRelation.Group))]
        public List<UserGroupRelation> Users { get; set; }
        public string GroupImage { get; set; }
        public string GroupName { get; set; }
        public override string GetDisplayImage(string userId) => this.GroupImage;
        public override string GetDisplayName(string userId) => this.GroupName;
        public override int GetUnReadAmount(string userId)
        {
            var relation = this.Users.SingleOrDefault(t => t.UserId == userId);
            return this.Messages.Where(t => t.SendTime > relation.ReadTimeStamp).Count();
        }

        public override Message GetLatestMessage()
        {
            try
            {
                return this.Messages.OrderByDescending(p => p.SendTime).First();
            }
            catch (InvalidOperationException)
            {
                return new Message
                {
                    Content = $"You have successfully joined {this.GroupName}!",
                    SendTime = this.ConversationCreateTime
                };
            }
        }
    }

    public class UserGroupRelation
    {
        [Key]
        public int Id { get; set; }
        public DateTime JoinTime { get; set; }

        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public KahlaUser User { get; set; }

        public int GroupId { get; set; }
        [ForeignKey(nameof(GroupId))]
        public GroupConversation Group { get; set; }

        public DateTime ReadTimeStamp { get; set; }
    }
}
