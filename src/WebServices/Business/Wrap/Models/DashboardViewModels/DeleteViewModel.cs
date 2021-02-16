using Aiursoft.Wrapgate.SDK.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Wrap.Models.DashboardViewModels
{
    public class DeleteViewModel : LayoutViewModel
    {
        public DeleteViewModel(WrapUser user) : base(user, "Delete record")
        {
        }

        [FromRoute]
        public string RecordName { get; set; }

        public void Recover(WrapUser user)
        {
            RootRecover(user, "Delete record");
        }
    }
}
