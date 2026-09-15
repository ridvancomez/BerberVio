using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using BerberVio.Areas.Cms.Controllers;
using BerberVio.Entities;
using BerberVio.Models;
using BerberVio.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BerberVio.Controllers;

public class HomeController : Controller
{
    private static readonly JsonSerializerOptions ApiJsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly ILogger<HomeController> _logger;
    private readonly ApiClient _apiClient;

    public HomeController(ILogger<HomeController> logger, ApiClient apiClient)
    {
        _logger = logger;
        _apiClient = apiClient;
    }

    public async Task<IActionResult> Index()
    {
        var services = await GetListAsync<Service>("api/service");
        return View(services);
    }

    [HttpGet]
    public async Task<IActionResult> MakeAppointment(AppointmentViewModel model)
    {
        await PopulateFormDataAsync(model);
        return View(model);
    }

    [HttpPost]
    [ActionName(nameof(MakeAppointment))]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MakeAppointmentPost(AppointmentViewModel model)
    {
        await PopulateFormDataAsync(model);

        if (model.AppointmentDate.Date < DateTime.Today)
        {
            ViewBag.Error = "Geçmiş bir tarih için randevu oluşturamazsınız.";
            return View(model);
        }

        if (model.EmployeeId <= 0 || model.ServiceId <= 0 || !AppointmentController.AllowedTimeSlots.Contains(model.AppointmentTime))
        {
            ViewBag.Error = "Lütfen çalışan, hizmet ve saat seçiniz.";
            return View(model);
        }

        var appointments = await GetListAsync<Appointment>("api/appointment");
        var requestedDateTime = model.AppointmentDate.Date + TimeSpan.Parse(model.AppointmentTime);

        var isTaken = appointments.Any(a => a.EmployeeId == model.EmployeeId && a.AppointmentDate == requestedDateTime);

        if (isTaken)
        {
            ViewBag.Error = "Bu çalışan seçilen saatte dolu, lütfen başka bir saat veya çalışan seçin.";
            return View(model);
        }

        var appointment = new Appointment
        {
            AppointmentDate = requestedDateTime,
            EmployeeId = model.EmployeeId,
            ServiceId = model.ServiceId
        };

        var response = await _apiClient.PostAsync("api/appointment", appointment);

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Randevu oluşturulurken bir hata oluştu.";
            return View(model);
        }

        return RedirectToAction(nameof(Confirmation));
    }

    public IActionResult Confirmation()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private async Task PopulateFormDataAsync(AppointmentViewModel model)
    {
        var employees = await GetListAsync<Employee>("api/employee");
        var services = await GetListAsync<Service>("api/service");

        ViewBag.Employees = new SelectList(employees, nameof(Employee.Id), nameof(Employee.Name), model.EmployeeId);
        ViewBag.Services = new SelectList(
            services.Select(s => new { s.Id, Display = $"{s.Name} - {s.Price:0.00} ₺" }),
            "Id", "Display", model.ServiceId);
        ViewBag.TimeSlots = AppointmentController.AllowedTimeSlots;
        ViewBag.BusySlots = await GetBusySlotsAsync(model.EmployeeId, model.AppointmentDate);
    }

    private async Task<List<string>> GetBusySlotsAsync(int employeeId, DateTime date)
    {
        if (employeeId <= 0)
        {
            return new List<string>();
        }

        var appointments = await GetListAsync<Appointment>("api/appointment");

        return appointments
            .Where(a => a.EmployeeId == employeeId && a.AppointmentDate.Date == date.Date)
            .Select(a => a.AppointmentDate.ToString("HH:mm"))
            .ToList();
    }

    private async Task<List<T>> GetListAsync<T>(string requestUri)
    {
        var response = await _apiClient.GetAsync(requestUri);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<List<T>>(ApiJsonOptions) ?? new List<T>()
            : new List<T>();
    }
}
