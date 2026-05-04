using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TransportPlatform.Accounting.Application.Interfaces;
using TransportPlatform.Accounting.Domain.Exceptions;

namespace TransportPlatform.Accounting.Infrastructure.Identity;

public class KeycloakIdentityService(
    IHttpClientFactory httpClientFactory,
    IConfiguration config,
    ILogger<KeycloakIdentityService> logger) : IIdentityService
{
    private readonly string _adminUrl  = config["Keycloak:AdminUrl"]      ?? "http://keycloak:8080";
    private readonly string _realm     = config["Keycloak:Realm"]         ?? "transport";
    private readonly string _clientId  = config["Keycloak:ClientId"]      ?? "transport-backoffice";
    private readonly string _adminUser = config["Keycloak:AdminUsername"] ?? "admin";
    private readonly string _adminPass = config["Keycloak:AdminPassword"] ?? "admin";

    public async Task<Guid> CreateUserAsync(string email, string password, CancellationToken ct = default)
    {
        var client = httpClientFactory.CreateClient("keycloak-admin");
        var token = await GetAdminTokenAsync(client, ct);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var payload = new
        {
            username      = email,
            email         = email,
            enabled       = true,
            emailVerified = true,
            credentials   = new[] { new { type = "password", value = password, temporary = false } }
        };

        var response = await client.PostAsJsonAsync($"{_adminUrl}/admin/realms/{_realm}/users", payload, ct);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct);
            logger.LogError("Keycloak user creation failed {Status}: {Body}", response.StatusCode, body);
            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                throw new AccountingDomainException($"An identity account for '{email}' already exists.");
            throw new AccountingDomainException("Failed to create identity account. Please try again later.");
        }

        var location = response.Headers.Location?.ToString()
            ?? throw new AccountingDomainException("Identity provider did not return a user ID.");
        return Guid.Parse(location.Split("/").Last());
    }

    public async Task<TokenResult> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        var client = httpClientFactory.CreateClient("keycloak-admin");
        var form = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"]  = _clientId,
            ["username"]   = email,
            ["password"]   = password,
            ["scope"]      = "openid profile email",
        };

        var response = await client.PostAsync(
            $"{_adminUrl}/realms/{_realm}/protocol/openid-connect/token",
            new FormUrlEncodedContent(form), ct);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct);
            logger.LogWarning("Keycloak login failed {Status}: {Body}", response.StatusCode, body);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new AccountingDomainException("Invalid email or password.");
            throw new AccountingDomainException("Authentication failed. Please try again later.");
        }

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
        return new TokenResult(
            AccessToken:  json.GetProperty("access_token").GetString()  ?? string.Empty,
            RefreshToken: json.GetProperty("refresh_token").GetString() ?? string.Empty,
            ExpiresIn:    json.GetProperty("expires_in").GetInt32(),
            TokenType:    json.GetProperty("token_type").GetString()    ?? "Bearer");
    }

    private async Task<string> GetAdminTokenAsync(HttpClient client, CancellationToken ct)
    {
        var form = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"]  = "admin-cli",
            ["username"]   = _adminUser,
            ["password"]   = _adminPass
        };

        var response = await client.PostAsync(
            $"{_adminUrl}/realms/master/protocol/openid-connect/token",
            new FormUrlEncodedContent(form), ct);

        if (!response.IsSuccessStatusCode)
            throw new AccountingDomainException("Could not authenticate with identity provider admin.");

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
        return json.GetProperty("access_token").GetString()
            ?? throw new AccountingDomainException("Identity provider returned an empty token.");
    }
}
