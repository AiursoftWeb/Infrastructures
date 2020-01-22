using Aiursoft.SDK.Models.API;
using Aiursoft.SDK.Models.Developer;
using Aiursoft.XelNaga.Models; using Aiursoft.Handler.Models;
using System;
using System.Collections.Generic;

namespace Aiursoft.Account.Models.AccountViewModels
{
    public class AuditLogViewModel : AccountViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public AuditLogViewModel()
        {
        }
        public AuditLogViewModel(AccountUser user) : base(user, "Audit Log") { }

        public List<App> Apps { get; set; } = new List<App>();
        public AiurPagedCollection<AuditLog> Logs { get; set; }
    }
}
