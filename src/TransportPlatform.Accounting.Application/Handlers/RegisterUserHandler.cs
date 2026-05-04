using Microsoft.Extensions.Logging;
using TransportPlatform.Accounting.Application.Commands;
using TransportPlatform.Accounting.Application.Interfaces;
using TransportPlatform.Accounting.Domain.Entities;
using TransportPlatform.Accounting.Domain.Exceptions;
using TransportPlatform.Accounting.Domain.Interfaces;
using TransportPlatform.Contracts.Events.Accounting;
using TransportPlatform.Contracts.Messaging;

namespace TransportPlatform.Accounting.Application.Handlers;

public class RegisterUserHandler(
    ICustomerRepository customers,
    IIdentityService identityService,
    IEventPublisher eventPublisher,
    ILogger<RegisterUserHandler> logger)
{
    public async Task<RegisterUserResult> HandleAsync(
        RegisterUserCommand command,
        CancellationToken ct = default)
    {
        var existing = await customers.GetByEmailAsync(command.Email, ct);
        if (existing is not null)
            throw new AccountingDomainException($"An account with email '{command.Email}' already exists.");

        // Create account in Keycloak (or any identity provider) first.
        // If this succeeds but DB insert fails, the identity account exists without a profile —
        // acceptable for demo; production would use a saga or idempotent retry.
        var identityId = await identityService.CreateUserAsync(
            command.Email,
            command.Password,
            ct);

        var customer = Customer.Create(
            command.FirstName,
            command.LastName,
            command.DateOfBirth,
            command.Email,
            identityId);

        await customers.AddAsync(customer, ct);

        await eventPublisher.PublishAsync(new CustomerRegistered(
            customer.Id,
            identityId,
            customer.FirstName,
            customer.LastName,
            customer.DateOfBirth,
            customer.Email,
            DateTimeOffset.UtcNow), ct);

        logger.LogInformation(
            "User registered: customer {CustomerId}, identity {IdentityId}",
            customer.Id, identityId);

        return new RegisterUserResult(customer.Id, identityId);
    }
}
