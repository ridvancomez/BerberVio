using System.Net.Http.Json;
using System.Text.Json;
using BerberVio.Entities;
using BerberVio.Models;
using BerberVio.Services;
using Microsoft.AspNetCore.Mvc;

namespace BerberVio.Areas.Cms.Controllers;

public class DashboardController : BaseController
{
    private static readonly JsonSerializerOptions ApiJsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly ApiClient _apiClient;

    public DashboardController(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IActionResult> Index()
    {
        var employees = await GetListAsync<Employee>("api/employee");
        var services = await GetListAsync<Service>("api/service");
        var appointments = await GetListAsync<Appointment>("api/appointment");

        var employeeNames = employees.ToDictionary(e => e.Id, e => e.Name);
        var serviceNames = services.ToDictionary(s => s.Id, s => s.Name);

        var todayAppointments = appointments
            .Where(a => a.AppointmentDate.Date == DateTime.Today)
            .OrderBy(a => a.AppointmentDate)
            .Select(a => new AppointmentViewModel
            {
                Id = a.Id,
                AppointmentDate = a.AppointmentDate,
                AppointmentTime = a.AppointmentDate.ToString("HH:mm"),
                EmployeeId = a.EmployeeId,
                ServiceId = a.ServiceId,
                EmployeeName = employeeNames.TryGetValue(a.EmployeeId, out var employeeName) ? employeeName : string.Empty,
                ServiceName = serviceNames.TryGetValue(a.ServiceId, out var serviceName) ? serviceName : string.Empty
            })
            .ToList();

        ViewBag.TotalEmployees = employees.Count;
        ViewBag.TotalServices = services.Count;
        ViewBag.TotalAppointments = appointments.Count;
        ViewBag.TodayAppointments = todayAppointments;

        return View();
    }

    private async Task<List<T>> GetListAsync<T>(string requestUri)
    {
        var response = await _apiClient.GetAsync(requestUri);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<List<T>>(ApiJsonOptions) ?? new List<T>()
            : new List<T>();
    }
}
