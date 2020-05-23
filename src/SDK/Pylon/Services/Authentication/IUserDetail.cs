using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Services.Authentication
{
    public interface IUserDetail
    {
        string Id { get; set; }
        string AvatarUrl { get; set; }

        [StringLength(maximumLength: 25, MinimumLength = 1)]
        [Required(ErrorMessage = "Nickname is required")]
        [Display(Name = "Nickname")]
        string Name { get; set; }
        string Bio { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        [Display(Name = "Your new account's Email")]
        string Email { get; set; }
    }
}
