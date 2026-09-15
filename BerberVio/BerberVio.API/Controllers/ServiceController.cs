using BerberVio.Business.Interfaces;
using BerberVio.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BerberVio.API.Controllers;

[ApiController]
[Route("api/service")]
[Authorize]
public class ServiceController : ControllerBase
{
    private readonly IServiceService _serviceService;

    public ServiceController(IServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetAll()
    {
        return Ok(_serviceService.GetAll());
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public IActionResult GetById(int id)
    {
        var service = _serviceService.GetById(id);
        return service == null ? NotFound() : Ok(service);
    }

    [HttpPost]
    public IActionResult Add(Service service)
    {
        _serviceService.Add(service);
        return CreatedAtAction(nameof(GetById), new { id = service.Id }, service);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Service service)
    {
        if (id != service.Id)
        {
            return BadRequest();
        }

        if (_serviceService.GetById(id) == null)
        {
            return NotFound();
        }

        _serviceService.Update(service);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (_serviceService.GetById(id) == null)
        {
            return NotFound();
        }

        _serviceService.Delete(id);
        return NoContent();
    }
}
