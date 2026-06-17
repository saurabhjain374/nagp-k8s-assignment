using EmployeeApi.Models;
using EmployeeApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly EmployeeRepository _repository;

    public EmployeesController(EmployeeRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetEmployees()
    {
        var result =
            await _repository.GetEmployees();

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployee(int id)
    {
        var employee =
            await _repository.GetEmployee(id);

        if (employee == null)
            return NotFound();

        return Ok(employee);
    }

    [HttpPost]
    public async Task<IActionResult> CreateEmployee(Employee employee)
    {
        var employeeId =
            await _repository.CreateEmployee(employee);

        return CreatedAtAction(
            nameof(GetEmployee),
            new { id = employeeId },
            new
            {
                employeeId,
                message = "Employee created successfully"
            });
    }

    [HttpGet("info")]
    public IActionResult GetAppInfo()
    {
        return Ok(new
        {
            Environment = Environment.GetEnvironmentVariable("APP_ENVIRONMENT") ?? "unknown",
            Version = Environment.GetEnvironmentVariable("APP_VERSION") ?? "unknown",
            Hostname = Environment.MachineName
        });
    }
}