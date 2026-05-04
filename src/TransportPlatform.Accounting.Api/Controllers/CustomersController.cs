using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransportPlatform.Accounting.Application.Commands;
using TransportPlatform.Accounting.Application.Handlers;
using TransportPlatform.Accounting.Application.Queries;
using TransportPlatform.Accounting.Domain.Exceptions;
using TransportPlatform.Infrastructure.Common.Auth;

namespace TransportPlatform.Accounting.Api.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize(Policy = "permission:accounting:write")]
public class CustomersController(
    CreateCustomerHandler createHandler,
    RegisterMyProfileHandler registerMyProfileHandler,
    UpdateMyProfileHandler updateMyProfileHandler,
    GetCustomersHandler getHandler,
    UserContext userContext) : ControllerBase
{
    /// <summary>List all customers (paged).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<CustomerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await getHandler.HandleAsync(new GetCustomersQuery(page, pageSize), ct);
        return Ok(result);
    }

    /// <summary>Get a single customer by ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        try
        {
            var result = await getHandler.HandleByIdAsync(new GetCustomerByIdQuery(id), ct);
            return Ok(result);
        }
        catch (CustomerNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
    }

    /// <summary>Create a customer profile linked to the authenticated user's identity.</summary>
    /// <remarks>
    /// Call this after the user has registered and obtained a JWT via Keycloak.
    /// The IdentityId is read automatically from the X-User-Id header injected by YARP — do not pass it in the body.
    /// </remarks>
    [HttpPost("me")]
    [ProducesResponseType(typeof(CreateCustomerResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterMyProfile(
        [FromBody] RegisterMyProfileRequest request,
        CancellationToken ct)
    {
        if (userContext.UserId is null)
            return Unauthorized(new { message = "No authenticated user identity found." });

        var result = await registerMyProfileHandler.HandleAsync(
            new RegisterMyProfileCommand(
                request.FirstName,
                request.LastName,
                request.DateOfBirth,
                request.Email,
                userContext.UserId.Value), ct);

        return CreatedAtAction(nameof(GetById), new { id = result.CustomerId }, result);
    }

    /// <summary>Update the authenticated user's profile (first name, last name, date of birth).</summary>
    [HttpPut("me")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMyProfile(
        [FromBody] UpdateMyProfileRequest request,
        CancellationToken ct)
    {
        if (userContext.UserId is null)
            return Unauthorized(new { message = "No authenticated user identity found." });

        await updateMyProfileHandler.HandleAsync(
            new UpdateMyProfileCommand(
                request.FirstName,
                request.LastName,
                request.DateOfBirth,
                userContext.UserId.Value), ct);

        return NoContent();
    }

    /// <summary>Create a customer record (admin use).</summary>
    /// <remarks>
    /// Optionally supply IdentityId to link the record to an existing identity provider account.
    /// Use POST /api/auth/register for self-service user registration instead.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(CreateCustomerResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCustomerRequest request,
        CancellationToken ct)
    {
        var result = await createHandler.HandleAsync(
            new CreateCustomerCommand(
                request.FirstName,
                request.LastName,
                request.DateOfBirth,
                request.Email,
                request.IdentityId), ct);

        return CreatedAtAction(nameof(GetById), new { id = result.CustomerId }, result);
    }
}

public record UpdateMyProfileRequest(
    string FirstName,
    string LastName,
    DateOnly DateOfBirth);

public record RegisterMyProfileRequest(
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string Email);

public record CreateCustomerRequest(
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string Email,
    Guid? IdentityId = null);
