using Aiursoft.Pylon.Models.API;
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
        public AuditLogViewModel(AccountUser user) : base(user, 6, "Audit Log") { }

        public List<AuditLog> Logs { get; set; }
    }
}
