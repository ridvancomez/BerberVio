using System.ComponentModel.DataAnnotations;

namespace BerberVio.Models;

public class AppointmentViewModel
{
    public int Id { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Tarih")]
    public DateTime AppointmentDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Saat seçiniz.")]
    [Display(Name = "Saat")]
    public string AppointmentTime { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Çalışan seçiniz.")]
    [Display(Name = "Çalışan")]
    public int EmployeeId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Hizmet seçiniz.")]
    [Display(Name = "Hizmet")]
    public int ServiceId { get; set; }

    [Display(Name = "Çalışan")]
    public string EmployeeName { get; set; } = string.Empty;

    [Display(Name = "Hizmet")]
    public string ServiceName { get; set; } = string.Empty;
}
