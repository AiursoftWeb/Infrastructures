namespace Aiursoft.Directory.Models.PasswordViewModels;

public class MethodSelectionViewModel
{
    public string AccountName { get; set; }
    public IEnumerable<UserEmail> AvailableEmails { get; set; }
    public bool SmsResetAvailable { get; set; }
    public string PhoneNumber { get; set; }
}