using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Aiursoft.Gateway.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GatewayUser : AiurUserBase
    {
        [InverseProperty(nameof(OAuthPack.User))]
        public IEnumerable<OAuthPack> Packs { get; set; }
        [InverseProperty(nameof(AppGrant.User))]
        public IEnumerable<AppGrant> GrantedApps { get; set; }
        [InverseProperty(nameof(UserEmail.Owner))]
        public IEnumerable<UserEmail> Emails { get; set; }
        [InverseProperty(nameof(AuditLogLocal.User))]
        public IEnumerable<AuditLogLocal> AuditLogs { get; set; }
        [InverseProperty(nameof(ThirdPartyAccount.Owner))]
        public IEnumerable<ThirdPartyAccount> ThirdPartyAccounts { get; set; }

        public string SMSPasswordResetToken { get; set; }

        /// <summary>
        /// Anti-spam usage. Private. Client IP address when creating this account.
        /// </summary>
        public string RegisterIPAddress { get; set; }

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
    }

    public class UserEmail : AiurUserEmail
    {
        [JsonIgnore]
        public string OwnerId { get; set; }
        [JsonIgnore]
        [ForeignKey(nameof(OwnerId))]
        public GatewayUser Owner { get; set; }
        [JsonIgnore]
        public string ValidateToken { get; set; }
        [JsonIgnore]
        public DateTime LastSendTime { get; set; } = DateTime.MinValue;
        [JsonIgnore]
        public int Priority { get; set; }
    }

    public class ThirdPartyAccount : AiurThirdPartyAccount
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonIgnore]
        public string OpenId { get; set; }
        [JsonIgnore]
        public string OwnerId { get; set; }
        [JsonIgnore]
        [ForeignKey(nameof(OwnerId))]
        public GatewayUser Owner { get; set; }
    }
}
