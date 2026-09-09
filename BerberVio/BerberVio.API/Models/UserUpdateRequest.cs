using System.ComponentModel.DataAnnotations;

namespace BerberVio.API.Models;

public class UserUpdateRequest
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Surname { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
}
