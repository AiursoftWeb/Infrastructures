namespace Aiursoft.Pylon.Services.Authentication
{
    public interface IUserDetail
    {
        public string Id { get; set; }
        public string AvatarUrl { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public string Email { get; set; }
    }
}
