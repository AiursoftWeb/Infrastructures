using System;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using Aiursoft.Pylon.Models.API.AccountViewModels;

namespace Aiursoft.Pylon.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class AiurUserBase : IdentityUser
    {
        public AiurUserBase() { }
        public AiurUserBase(UserInfoViewModel model)
        {
            Update(model);
        }

        public void Update(UserInfoViewModel model)
        {
            Id = model.User.Id;
            NickName = model.User.NickName;
            Sex = model.User.Sex;
            HeadImgFileKey = model.User.HeadImgFileKey;
            PreferedLanguage = model.User.PreferedLanguage;
            AccountCreateTime = model.User.AccountCreateTime;
            UserName = model.User.Email;
            Email = model.User.Email;
            Bio = model.User.Bio;
            EmailConfirmed = model.User.EmailConfirmed;
        }

        [JsonProperty]
        public override string Id { get => base.Id; set => base.Id = value; }
        [JsonProperty]
        public virtual string Bio { get; set; }
        [JsonProperty]
        public virtual string NickName { get; set; }
        [JsonProperty]
        public virtual string Sex { get; set; }
        [JsonProperty]
        public virtual int HeadImgFileKey { get; set; }
        [JsonProperty]
        public virtual string PreferedLanguage { get; set; } = "UnSet";
        [JsonProperty]
        public virtual DateTime AccountCreateTime { get; set; } = DateTime.UtcNow;

        [JsonProperty]
        public override bool EmailConfirmed { get; set; }
        [JsonProperty]
        public override string Email { get; set; }
        [NotMapped]
        public override bool PhoneNumberConfirmed => !string.IsNullOrEmpty(PhoneNumber);
    }
}
