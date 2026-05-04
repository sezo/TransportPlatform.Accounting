namespace TransportPlatform.Accounting.Application.Commands;

public record RegisterUserCommand(
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string Email,
    string Password);

public record RegisterUserResult(Guid CustomerId, Guid IdentityId);
