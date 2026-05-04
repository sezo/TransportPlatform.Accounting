using Microsoft.Extensions.Logging;
using TransportPlatform.Accounting.Application.Commands;
using TransportPlatform.Accounting.Domain.Entities;
using TransportPlatform.Accounting.Domain.Exceptions;
using TransportPlatform.Accounting.Domain.Interfaces;
using TransportPlatform.Contracts.Events.Accounting;
using TransportPlatform.Contracts.Messaging;

namespace TransportPlatform.Accounting.Application.Handlers;

public class CreateCustomerHandler(
    ICustomerRepository customers,
    IEventPublisher eventPublisher,
    ILogger<CreateCustomerHandler> logger)
{
    public async Task<CreateCustomerResult> HandleAsync(
        CreateCustomerCommand command,
        CancellationToken ct = default)
    {
        var existing = await customers.GetByEmailAsync(command.Email, ct);
        if (existing is not null)
            throw new AccountingDomainException($"A customer with email '{command.Email}' already exists.");

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

        logger.LogInformation("Customer {CustomerId} created: {FullName}",
            customer.Id, customer.FullName);

        return new CreateCustomerResult(customer.Id);
    }
}
