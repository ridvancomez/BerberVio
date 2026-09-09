using System.ComponentModel.DataAnnotations;

namespace BerberVio.Models;

public class EmployeeViewModel
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Ad")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Telefon")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Display(Name = "Fotoğraf")]
    public IFormFile? ImageFile { get; set; }

    public string? ExistingImagePath { get; set; }
}
