using System.Collections.Generic;

namespace Aiursoft.API.Models.PasswordViewModels
{
    public class MethodSelectionViewModel
    {
        public string AccountName { get; set; }
        public IEnumerable<UserEmail> AvaliableEmails { get; set; }
        public bool SMSResetAvaliable { get; set; }
        public string PhoneNumber { get; set; }
    }
}
