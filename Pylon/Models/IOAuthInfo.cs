namespace Aiursoft.Pylon.Models
{
    public interface IAuthorizeViewModel
    {
        string State { get; set; }
        string ToRedirect { get; set; }
        string Email { get; set; }
        string AppId { get; set; }
        string GetRegexRedirectUrl();
        string Scope { get; set; }
        string ResponseType { get; set; }
    }
}
