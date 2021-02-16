using Aiursoft.Wrapgate.SDK.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Wrap.Models.DashboardViewModels
{
    public class EditViewModel : LayoutViewModel
    {
        public EditViewModel(WrapUser user) : base(user, "Edit record")
        {
        }

        public void Recover(WrapUser user)
        {
            RootRecover(user, "Edit record");
        }

        [FromRoute]
        public string RecordName { get; set; }
        public string NewRecordName { get; set; }
        public RecordType Type { get; set; }
        public string URL { get; set; }
        public bool Enabled { get; set; }
    }
}
