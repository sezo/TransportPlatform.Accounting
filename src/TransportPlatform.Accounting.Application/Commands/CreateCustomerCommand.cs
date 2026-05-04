namespace TransportPlatform.Accounting.Application.Commands;

public record CreateCustomerCommand(
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string Email,
    Guid? IdentityId = null);

// Used when an authenticated user registers their own profile.
// IdentityId is taken from X-User-Id header — not provided by the caller.
public record RegisterMyProfileCommand(
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string Email,
    Guid IdentityId);

public record CreateCustomerResult(Guid CustomerId);
