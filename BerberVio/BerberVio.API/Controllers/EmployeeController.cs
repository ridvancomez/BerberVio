using BerberVio.Business.Interfaces;
using BerberVio.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BerberVio.API.Controllers;

[ApiController]
[Route("api/employee")]
[Authorize]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeeController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_employeeService.GetAll());
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var employee = _employeeService.GetById(id);
        return employee == null ? NotFound() : Ok(employee);
    }

    [HttpPost]
    public IActionResult Add(Employee employee)
    {
        _employeeService.Add(employee);
        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Employee employee)
    {
        if (id != employee.Id)
        {
            return BadRequest();
        }

        if (_employeeService.GetById(id) == null)
        {
            return NotFound();
        }

        _employeeService.Update(employee);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (_employeeService.GetById(id) == null)
        {
            return NotFound();
        }

        _employeeService.Delete(id);
        return NoContent();
    }
}
