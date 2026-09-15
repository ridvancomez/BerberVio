using System.ComponentModel.DataAnnotations;

namespace BerberVio.Models;

public class ServiceViewModel
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Ad")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Fiyat")]
    public decimal Price { get; set; }
}
