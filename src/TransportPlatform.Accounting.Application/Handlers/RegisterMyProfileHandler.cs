using Microsoft.Extensions.Logging;
using TransportPlatform.Accounting.Application.Commands;
using TransportPlatform.Accounting.Domain.Entities;
using TransportPlatform.Accounting.Domain.Exceptions;
using TransportPlatform.Accounting.Domain.Interfaces;
using TransportPlatform.Contracts.Events.Accounting;
using TransportPlatform.Contracts.Messaging;

namespace TransportPlatform.Accounting.Application.Handlers;

/// <summary>
/// Self-service: authenticated user completes their profile.
/// IdentityId comes from the X-User-Id header — the caller never supplies it.
/// </summary>
public class RegisterMyProfileHandler(
    ICustomerRepository customers,
    IEventPublisher eventPublisher,
    ILogger<RegisterMyProfileHandler> logger)
{
    public async Task<CreateCustomerResult> HandleAsync(
        RegisterMyProfileCommand command,
        CancellationToken ct = default)
    {
        var existing = await customers.GetByIdentityIdAsync(command.IdentityId, ct);
        if (existing is not null)
            throw new AccountingDomainException("A profile already exists for this account.");

        var byEmail = await customers.GetByEmailAsync(command.Email, ct);
        if (byEmail is not null)
            throw new AccountingDomainException($"Email '{command.Email}' is already registered.");

        var customer = Customer.Create(
            command.FirstName,
            command.LastName,
            command.DateOfBirth,
            command.Email,
            command.IdentityId);

        await customers.AddAsync(customer, ct);

        await eventPublisher.PublishAsync(new CustomerRegistered(
            customer.Id,
            customer.IdentityId,
            customer.FirstName,
            customer.LastName,
            customer.DateOfBirth,
            customer.Email,
            DateTimeOffset.UtcNow), ct);

        logger.LogInformation("Profile created for identity {IdentityId} → customer {CustomerId}",
            command.IdentityId, customer.Id);

        return new CreateCustomerResult(customer.Id);
    }
}