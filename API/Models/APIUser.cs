using Aiursoft.API.Data;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.API;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.API.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class APIUser : AiurUserBase
    {
        [InverseProperty(nameof(OAuthPack.User))]
        public List<OAuthPack> Packs { get; set; }
        [InverseProperty(nameof(AppGrant.User))]
        public List<AppGrant> GrantedApps { get; set; }
        [InverseProperty(nameof(UserEmail.Owner))]
        public List<UserEmail> Emails { get; set; }
        public virtual string SMSPasswordResetToken { get; set; }

        [JsonProperty]
        [NotMapped]
        public override bool EmailConfirmed => Emails?.Any(t => t.Validated) ?? false;
        [JsonProperty]
        [NotMapped]
        public override string Email => Emails?
            .OrderByDescending(t => t.Validated)
            .ThenByDescending(t => t.Priority)
            .First()?
            .EmailAddress ?? string.Empty;

        public async virtual Task GrantTargetApp(APIDbContext dbContext, string appId)
        {
            if (!await HasAuthorizedApp(dbContext, appId))
            {
                var appGrant = new AppGrant
                {
                    AppID = appId,
                    APIUserId = Id
                };
                dbContext.LocalAppGrant.Add(appGrant);
                await dbContext.SaveChangesAsync();
            }
        }

        public async virtual Task<OAuthPack> GeneratePack(APIDbContext dbContext, string appId)
        {
            var pack = new OAuthPack
            {
                Code = Math.Abs(Guid.NewGuid().GetHashCode()),
                UserId = Id,
                ApplyAppId = appId
            };
            dbContext.OAuthPack.Add(pack);
            await dbContext.SaveChangesAsync();
            return pack;
        }

        public async Task<bool> HasAuthorizedApp(APIDbContext dbContext, string appId)
        {
            return await dbContext.LocalAppGrant.AnyAsync(t => t.AppID == appId && t.APIUserId == Id);
        }
    }

    public class UserEmail : AiurUserEmail
    {
        [JsonIgnore]
        public string OwnerId { get; set; }
        [JsonIgnore]
        [ForeignKey(nameof(OwnerId))]
        public APIUser Owner { get; set; }
        [JsonIgnore]
        public string ValidateToken { get; set; }
        [JsonIgnore]
        public DateTime LastSendTime { get; set; } = DateTime.MinValue;
        [JsonIgnore]
        public int Priority { get; set; }
    }
}
