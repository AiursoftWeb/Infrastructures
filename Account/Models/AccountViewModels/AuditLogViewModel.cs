using Aiursoft.Pylon.Models.API;
using Aiursoft.Pylon.Models.Developer;
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

        public List<AuditLog> Logs { get; set; }
        public List<App> Apps { get; set; } = new List<App>();

        public int PageIndex { get; set; }
        public int TotalCount { get; set; }
    }
}
