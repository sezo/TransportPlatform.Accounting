using TransportPlatform.Accounting.Application.Commands;
using TransportPlatform.Accounting.Application.Interfaces;

namespace TransportPlatform.Accounting.Application.Handlers;

public class LoginHandler(IIdentityService identityService)
{
    public Task<TokenResult> HandleAsync(LoginCommand command, CancellationToken ct = default)
        => identityService.LoginAsync(command.Email, command.Password, ct);
}