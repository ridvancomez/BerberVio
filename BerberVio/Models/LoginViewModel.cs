using System.ComponentModel.DataAnnotations;

namespace BerberVio.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Valid email is required: ex@abc.xyz")]
    [EmailAddress(ErrorMessage = "Valid email is required: ex@abc.xyz")]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = null!;
}
