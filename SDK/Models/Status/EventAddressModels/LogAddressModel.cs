namespace Aiursoft.SDK.Models.Status.EventAddressModels
{
    public class LogAddressModel
    {
        public string AccessToken { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string Path { get; set; }
        public EventLevel EventLevel { get; set; }
    }
}
