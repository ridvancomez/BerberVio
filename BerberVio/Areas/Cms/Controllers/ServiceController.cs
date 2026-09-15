using System.Net.Http.Json;
using System.Text.Json;
using BerberVio.Entities;
using BerberVio.Models;
using BerberVio.Services;
using Microsoft.AspNetCore.Mvc;

namespace BerberVio.Areas.Cms.Controllers;

public class ServiceController : BaseController
{
    private static readonly JsonSerializerOptions ApiJsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly ApiClient _apiClient;

    public ServiceController(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IActionResult> Index()
    {
        var response = await _apiClient.GetAsync("api/service");

        var services = response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<List<Service>>(ApiJsonOptions)
            : new List<Service>();

        return View(services ?? new List<Service>());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new ServiceViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ServiceViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var service = new Service
        {
            Name = model.Name,
            Price = model.Price
        };

        var response = await _apiClient.PostAsync("api/service", service);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Hizmet oluşturulurken bir hata oluştu.");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var response = await _apiClient.GetAsync($"api/service/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return NotFound();
        }

        var service = await response.Content.ReadFromJsonAsync<Service>(ApiJsonOptions);

        if (service == null)
        {
            return NotFound();
        }

        var model = new ServiceViewModel
        {
            Id = service.Id,
            Name = service.Name,
            Price = service.Price
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ServiceViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var service = new Service
        {
            Id = model.Id,
            Name = model.Name,
            Price = model.Price
        };

        var response = await _apiClient.PutAsync($"api/service/{id}", service);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Hizmet güncellenirken bir hata oluştu.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _apiClient.DeleteAsync($"api/service/{id}");
        return RedirectToAction(nameof(Index));
    }
}
