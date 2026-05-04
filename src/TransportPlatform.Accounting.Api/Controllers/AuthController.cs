using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransportPlatform.Accounting.Application.Commands;
using TransportPlatform.Accounting.Application.Handlers;

namespace TransportPlatform.Accounting.Api.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController(RegisterUserHandler registerHandler) : ControllerBase
{
    /// <summary>Register a new user account.</summary>
    /// <remarks>
    /// Creates an identity account in Keycloak and a customer profile in the accounting database in a single call.
    /// No authentication required — this is the entry point for new app users.
    /// </remarks>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterUserResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var result = await registerHandler.HandleAsync(
            new RegisterUserCommand(
                request.FirstName,
                request.LastName,
                request.DateOfBirth,
                request.Email,
                request.Password), ct);

        return Created(string.Empty, new
        {
            result.CustomerId,
            result.IdentityId,
            message = "Registration successful. You can now log in with your email and password."
        });
    }
}

public record RegisterRequest(
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string Email,
    string Password);
