using System.Net.Http.Json;
using System.Text.Json;
using BerberVio.Entities;
using BerberVio.Models;
using BerberVio.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BerberVio.Areas.Cms.Controllers;

public class AppointmentController : BaseController
{
    private static readonly JsonSerializerOptions ApiJsonOptions = new() { PropertyNameCaseInsensitive = true };

    public static readonly string[] AllowedTimeSlots =
    {
        "12:00", "13:00", "14:00", "15:00", "16:00", "17:00", "18:00", "19:00", "20:00"
    };

    private readonly ApiClient _apiClient;

    public AppointmentController(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IActionResult> Index()
    {
        var response = await _apiClient.GetAsync("api/appointment");

        var appointments = response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<List<Appointment>>(ApiJsonOptions) ?? new List<Appointment>()
            : new List<Appointment>();

        var employees = await GetEmployeesAsync();
        var services = await GetServicesAsync();

        var employeeNames = employees.ToDictionary(e => e.Id, e => e.Name);
        var serviceNames = services.ToDictionary(s => s.Id, s => s.Name);

        var model = appointments.Select(a => new AppointmentViewModel
        {
            Id = a.Id,
            AppointmentDate = a.AppointmentDate,
            EmployeeId = a.EmployeeId,
            ServiceId = a.ServiceId,
            EmployeeName = employeeNames.TryGetValue(a.EmployeeId, out var employeeName) ? employeeName : string.Empty,
            ServiceName = serviceNames.TryGetValue(a.ServiceId, out var serviceName) ? serviceName : string.Empty
        }).ToList();

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new AppointmentViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AppointmentViewModel model)
    {
        ValidateTimeSlot(model);

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(model.EmployeeId, model.ServiceId);
            return View(model);
        }

        var appointment = new Appointment
        {
            AppointmentDate = CombineDateAndTime(model.AppointmentDate, model.AppointmentTime),
            EmployeeId = model.EmployeeId,
            ServiceId = model.ServiceId
        };

        var response = await _apiClient.PostAsync("api/appointment", appointment);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Randevu oluşturulurken bir hata oluştu.");
        await PopulateDropdownsAsync(model.EmployeeId, model.ServiceId);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var response = await _apiClient.GetAsync($"api/appointment/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return NotFound();
        }

        var appointment = await response.Content.ReadFromJsonAsync<Appointment>(ApiJsonOptions);

        if (appointment == null)
        {
            return NotFound();
        }

        var model = new AppointmentViewModel
        {
            Id = appointment.Id,
            AppointmentDate = appointment.AppointmentDate.Date,
            AppointmentTime = appointment.AppointmentDate.ToString("HH:mm"),
            EmployeeId = appointment.EmployeeId,
            ServiceId = appointment.ServiceId
        };

        await PopulateDropdownsAsync(model.EmployeeId, model.ServiceId);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AppointmentViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        ValidateTimeSlot(model);

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(model.EmployeeId, model.ServiceId);
            return View(model);
        }

        var appointment = new Appointment
        {
            Id = model.Id,
            AppointmentDate = CombineDateAndTime(model.AppointmentDate, model.AppointmentTime),
            EmployeeId = model.EmployeeId,
            ServiceId = model.ServiceId
        };

        var response = await _apiClient.PutAsync($"api/appointment/{id}", appointment);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Randevu güncellenirken bir hata oluştu.");
        await PopulateDropdownsAsync(model.EmployeeId, model.ServiceId);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _apiClient.DeleteAsync($"api/appointment/{id}");
        return RedirectToAction(nameof(Index));
    }

    private void ValidateTimeSlot(AppointmentViewModel model)
    {
        if (!AllowedTimeSlots.Contains(model.AppointmentTime))
        {
            ModelState.AddModelError(nameof(model.AppointmentTime), "Geçersiz randevu saati.");
        }
    }

    private static DateTime CombineDateAndTime(DateTime date, string time)
    {
        return date.Date + TimeSpan.Parse(time);
    }

    private async Task<List<Employee>> GetEmployeesAsync()
    {
        var response = await _apiClient.GetAsync("api/employee");
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<List<Employee>>(ApiJsonOptions) ?? new List<Employee>()
            : new List<Employee>();
    }

    private async Task<List<Service>> GetServicesAsync()
    {
        var response = await _apiClient.GetAsync("api/service");
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<List<Service>>(ApiJsonOptions) ?? new List<Service>()
            : new List<Service>();
    }

    private async Task PopulateDropdownsAsync(int? selectedEmployeeId = null, int? selectedServiceId = null)
    {
        var employees = await GetEmployeesAsync();
        var services = await GetServicesAsync();

        ViewBag.Employees = new SelectList(employees, nameof(Employee.Id), nameof(Employee.Name), selectedEmployeeId);
        ViewBag.Services = new SelectList(services, nameof(Service.Id), nameof(Service.Name), selectedServiceId);
        ViewBag.TimeSlots = AllowedTimeSlots;
    }
}
