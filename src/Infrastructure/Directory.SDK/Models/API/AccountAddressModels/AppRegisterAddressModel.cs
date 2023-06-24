using System.ComponentModel.DataAnnotations;
using Aiursoft.Directory.SDK.Attributes;
using Aiursoft.CSTools.Attributes;

namespace Aiursoft.Directory.SDK.Models.API.AccountAddressModels;

public class AppRegisterAddressModel
{
    [Required] 
    [IsAccessToken]
    public string AccessToken { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
        MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    [NoSpace]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}