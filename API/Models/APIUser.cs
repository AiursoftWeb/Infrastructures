using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Aiursoft.API.Data;
using Aiursoft.Pylon.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Aiursoft.Pylon.Models.API;
using System.ComponentModel.DataAnnotations;

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

        public async virtual Task GrantTargetApp(APIDbContext dbContext, string appId)
        {
            if (!await HasAuthorizedApp(dbContext, appId))
            {
                var AppGrant = new AppGrant
                {
                    AppID = appId,
                    APIUserId = Id,
                };
                dbContext.LocalAppGrant.Add(AppGrant);
                await dbContext.SaveChangesAsync();
            }
        }

        public async virtual Task<OAuthPack> GeneratePack(APIDbContext dbContext, string appId)
        {
            var pack = new OAuthPack
            {
                Code = Guid.NewGuid().GetHashCode(),
                UserId = Id,
                ApplyAppId = appId
            };
            dbContext.OAuthPack.Add(pack);
            await dbContext.SaveChangesAsync();
            return pack;
        }

        public async Task<bool> HasAuthorizedApp(APIDbContext DbContext, string appId)
        {
            var appGrant = await DbContext.LocalAppGrant.SingleOrDefaultAsync(t => t.AppID == appId && t.APIUserId == this.Id);
            return appGrant != null;
        }
    }

    public class UserEmail : AiurUserEmail
    {
        [JsonIgnore]
        public string OwnerId { get; set; }
        [JsonIgnore]
        [ForeignKey(nameof(OwnerId))]
        public APIUser Owner { get; set; }

        public string ValidateToken { get; set; }
    }
}
