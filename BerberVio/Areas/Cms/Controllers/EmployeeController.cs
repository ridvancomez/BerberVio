using System.Net.Http.Json;
using System.Text.Json;
using BerberVio.Entities;
using BerberVio.Models;
using BerberVio.Services;
using Microsoft.AspNetCore.Mvc;

namespace BerberVio.Areas.Cms.Controllers;

public class EmployeeController : BaseController
{
    private static readonly JsonSerializerOptions ApiJsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly ApiClient _apiClient;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public EmployeeController(ApiClient apiClient, IWebHostEnvironment webHostEnvironment)
    {
        _apiClient = apiClient;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IActionResult> Index()
    {
        var response = await _apiClient.GetAsync("api/employee");

        var employees = response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<List<Employee>>(ApiJsonOptions)
            : new List<Employee>();

        return View(employees ?? new List<Employee>());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new EmployeeViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var employee = new Employee
        {
            Name = model.Name,
            PhoneNumber = model.PhoneNumber,
            ImagePath = await SaveImageAsync(model.ImageFile)
        };

        var response = await _apiClient.PostAsync("api/employee", employee);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Çalışan oluşturulurken bir hata oluştu.");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var response = await _apiClient.GetAsync($"api/employee/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return NotFound();
        }

        var employee = await response.Content.ReadFromJsonAsync<Employee>(ApiJsonOptions);

        if (employee == null)
        {
            return NotFound();
        }

        var model = new EmployeeViewModel
        {
            Id = employee.Id,
            Name = employee.Name,
            PhoneNumber = employee.PhoneNumber,
            ExistingImagePath = employee.ImagePath
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EmployeeViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var employee = new Employee
        {
            Id = model.Id,
            Name = model.Name,
            PhoneNumber = model.PhoneNumber,
            ImagePath = model.ImageFile != null ? await SaveImageAsync(model.ImageFile) : model.ExistingImagePath
        };

        var response = await _apiClient.PutAsync($"api/employee/{id}", employee);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Çalışan güncellenirken bir hata oluştu.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _apiClient.DeleteAsync($"api/employee/{id}");
        return RedirectToAction(nameof(Index));
    }

    private async Task<string?> SaveImageAsync(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            return null;
        }

        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "employees");
        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/uploads/employees/{fileName}";
    }
}
