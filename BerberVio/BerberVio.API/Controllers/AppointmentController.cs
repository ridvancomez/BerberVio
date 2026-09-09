using BerberVio.Business.Interfaces;
using BerberVio.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BerberVio.API.Controllers;

[ApiController]
[Route("api/appointment")]
[Authorize]
public class AppointmentController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_appointmentService.GetAll());
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var appointment = _appointmentService.GetById(id);
        return appointment == null ? NotFound() : Ok(appointment);
    }

    [HttpPost]
    public IActionResult Add(Appointment appointment)
    {
        _appointmentService.Add(appointment);
        return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Appointment appointment)
    {
        if (id != appointment.Id)
        {
            return BadRequest();
        }

        if (_appointmentService.GetById(id) == null)
        {
            return NotFound();
        }

        _appointmentService.Update(appointment);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (_appointmentService.GetById(id) == null)
        {
            return NotFound();
        }

        _appointmentService.Delete(id);
        return NoContent();
    }
}
