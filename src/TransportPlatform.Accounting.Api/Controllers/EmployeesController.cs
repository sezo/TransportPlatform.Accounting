using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransportPlatform.Accounting.Application.Commands;
using TransportPlatform.Accounting.Application.Handlers;
using TransportPlatform.Accounting.Application.Queries;
using TransportPlatform.Accounting.Domain.Exceptions;

namespace TransportPlatform.Accounting.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/employees")]
[Authorize(Policy = "permission:accounting:write")]
public class EmployeesController(
    CreateEmployeeHandler createHandler,
    GetEmployeesHandler getHandler) : ControllerBase
{
    /// <summary>List all employees (paged).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<EmployeeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await getHandler.HandleAsync(new GetEmployeesQuery(page, pageSize), ct);
        return Ok(result);
    }

    /// <summary>Get a single employee by ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        try
        {
            var result = await getHandler.HandleByIdAsync(new GetEmployeeByIdQuery(id), ct);
            return Ok(result);
        }
        catch (EmployeeNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
    }

    /// <summary>Create a new employee record.</summary>
    /// <remarks>
    /// Optionally supply IdentityId (UUID from Keycloak admin console) to link the employee
    /// to an existing identity provider account, enabling them to log in.
    /// If omitted, the employee record exists without login access until linked later.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(CreateEmployeeResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        [FromBody] CreateEmployeeRequest request,
        CancellationToken ct)
    {
        var result = await createHandler.HandleAsync(
            new CreateEmployeeCommand(
                request.FirstName,
                request.LastName,
                request.DateOfBirth,
                request.Email,
                request.Position,
                request.Department,
                request.HireDate,
                request.Salary,
                request.IdentityId), ct);

        return CreatedAtAction(nameof(GetById), new { id = result.EmployeeId }, result);
    }
}

public record CreateEmployeeRequest(
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string Email,
    string Position,
    string Department,
    DateOnly HireDate,
    decimal Salary,
    Guid? IdentityId = null);
