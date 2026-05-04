using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransportPlatform.Accounting.Application.Commands;
using TransportPlatform.Accounting.Application.Handlers;
using TransportPlatform.Accounting.Domain.Exceptions;

namespace TransportPlatform.Accounting.Api.Controllers;

[ApiController]
[Route("api/accounting/auth")]
[AllowAnonymous]
public class AuthController(
    RegisterUserHandler registerHandler,
    LoginHandler loginHandler) : ControllerBase
{
    /// <summary>Register a new user account.</summary>
    /// <remarks>
    /// Creates an identity account in Keycloak and a customer profile in the accounting
    /// database in a single call. No authentication required.
    /// </remarks>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
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

    /// <summary>Authenticate with email and password.</summary>
    /// <remarks>
    /// Proxies credentials to Keycloak server-side via ROPC. Keycloak is never exposed to the client.
    /// </remarks>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        try
        {
            var tokens = await loginHandler.HandleAsync(
                new LoginCommand(request.Email, request.Password), ct);

            return Ok(new
            {
                access_token  = tokens.AccessToken,
                refresh_token = tokens.RefreshToken,
                expires_in    = tokens.ExpiresIn,
                token_type    = tokens.TokenType,
            });
        }
        catch (AccountingDomainException ex)
        {
            return Unauthorized(new ProblemDetails
            {
                Title  = "Authentication failed",
                Detail = ex.Message,
                Status = StatusCodes.Status401Unauthorized,
            });
        }
    }
}

public record RegisterRequest(
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string Email,
    string Password);

public record LoginRequest(string Email, string Password);