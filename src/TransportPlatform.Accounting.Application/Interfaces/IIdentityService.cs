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

    /// <summary>
    /// Authenticates a user via the identity provider using Resource Owner Password Credentials.
    /// Returns the raw token response (access_token, refresh_token, expires_in, etc.).
    /// </summary>
    Task<TokenResult> LoginAsync(
        string email,
        string password,
        CancellationToken ct = default);
}

public record TokenResult(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    string TokenType);