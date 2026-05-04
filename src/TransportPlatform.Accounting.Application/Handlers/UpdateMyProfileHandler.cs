using Microsoft.Extensions.Logging;
using TransportPlatform.Accounting.Application.Commands;
using TransportPlatform.Accounting.Domain.Exceptions;
using TransportPlatform.Accounting.Domain.Interfaces;
using TransportPlatform.Contracts.Events.Accounting;
using TransportPlatform.Contracts.Messaging;

namespace TransportPlatform.Accounting.Application.Handlers;

public class UpdateMyProfileHandler(
    ICustomerRepository customers,
    IEventPublisher eventPublisher,
    ILogger<UpdateMyProfileHandler> logger)
{
    public async Task HandleAsync(
        UpdateMyProfileCommand command,
        CancellationToken ct = default)
    {
        var customer = await customers.GetByIdentityIdAsync(command.IdentityId, ct)
            ?? throw new AccountingDomainException("No profile found for this account.");

        customer.Update(command.FirstName, command.LastName, command.DateOfBirth);
        await customers.UpdateAsync(customer, ct);

        await eventPublisher.PublishAsync(new CustomerUpdated(
            customer.Id,
            customer.IdentityId,
            customer.FirstName,
            customer.LastName,
            customer.DateOfBirth,
            customer.Email,
            DateTimeOffset.UtcNow), ct);

        logger.LogInformation("Profile updated for identity {IdentityId} → customer {CustomerId}",
            command.IdentityId, customer.Id);
    }
}
