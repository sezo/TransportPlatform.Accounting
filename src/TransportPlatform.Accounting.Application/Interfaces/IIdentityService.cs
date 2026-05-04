namespace TransportPlatform.Accounting.Application.Interfaces;

public interface IIdentityService
{
    /// <summary>
    /// Creates a user account in the identity provider.
    /// Returns the provider-assigned user ID (IdentityId).
    /// </summary>
    Task<Guid> CreateUserAsync(
        string email,
        string password,
        CancellationToken ct = default);
}
