using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Services.Authentication
{
    public interface IUserDetail
    {
        public string Id { get; set; }
        public string AvatarUrl { get; set; }

        [StringLength(maximumLength: 25, MinimumLength = 1)]
        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Nick name")]
        public string Name { get; set; }
        public string Bio { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        [Display(Name = "Your new account's Email")]
        public string Email { get; set; }
    }
}
