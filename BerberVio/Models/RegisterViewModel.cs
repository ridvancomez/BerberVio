using System.ComponentModel.DataAnnotations;

namespace BerberVio.Models;

public class RegisterViewModel
{
    [Required]
    [Display(Name = "Name")]
    public string Name { get; set; } = null!;

    [Required]
    [Display(Name = "Surname")]
    public string Surname { get; set; } = null!;

    [Required]
    [Phone]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = null!;

    [Required]
    [EmailAddress]
    [Display(Name = "Mail")]
    public string Email { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = null!;
}
