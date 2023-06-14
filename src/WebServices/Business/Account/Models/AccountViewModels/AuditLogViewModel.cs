using System;
using System.Collections.Concurrent;
using Aiursoft.Directory.SDK.Models;
using Aiursoft.Directory.SDK.Models.API;
using Aiursoft.Handler.Models;

namespace Aiursoft.Account.Models.AccountViewModels;

public class AuditLogViewModel : AccountViewModel
{
    [Obsolete(error: true, message: "This method is only for framework!")]
    public AuditLogViewModel()
    {
    }

    public AuditLogViewModel(AccountUser user) : base(user, "Audit Log")
    {
    }

    public ConcurrentBag<DirectoryApp> Apps { get; set; } = new();
    public AiurPagedCollection<AuditLog> Logs { get; set; }
}