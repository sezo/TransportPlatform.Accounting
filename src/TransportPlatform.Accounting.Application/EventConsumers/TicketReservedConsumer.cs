using MassTransit;
using Microsoft.Extensions.Logging;
using TransportPlatform.Accounting.Domain.Entities;
using TransportPlatform.Accounting.Domain.Enums;
using TransportPlatform.Accounting.Domain.Interfaces;
using TransportPlatform.Contracts.Events.Accounting;
using TransportPlatform.Contracts.Events.Ticketing;
using TransportPlatform.Contracts.Messaging;

namespace TransportPlatform.Accounting.Application.EventConsumers;

/// <summary>
/// Responds to TicketReserved from the saga:
///   1. Creates a pending ledger entry for the reservation
///   2. Simulates payment processing (always succeeds in demo)
///   3. Publishes PaymentProcessed → saga advances to CapacityReserved wait
/// </summary>
public class TicketReservedConsumer(
    ILedgerRepository ledger,
    IEventPublisher publisher,
    ILogger<TicketReservedConsumer> logger) : IConsumer<TicketReserved>
{
    public async Task Consume(ConsumeContext<TicketReserved> context)
    {
        var e = context.Message;
        logger.LogInformation("Processing payment for ticket {TicketId}, amount {Amount}",
            e.TicketId, e.Price);

        var invoiceId = Guid.NewGuid();

        var entry = LedgerEntry.CreatePending(
            ticketId: e.TicketId,
            invoiceId: invoiceId,
            amount: e.Price,
            type: EntryType.Credit,
            description: $"Ticket sale — route {e.RouteId}, seat {e.SeatNumber}");

        await ledger.AddAsync(entry, context.CancellationToken);

        // Simulate payment gateway (always succeeds for demo)
        await Task.Delay(50, context.CancellationToken);

        entry.Confirm();
        await ledger.UpdateAsync(entry, context.CancellationToken);

        await publisher.PublishAsync(new PaymentProcessed(
            TicketId: e.TicketId,
            InvoiceId: invoiceId,
            Amount: e.Price,
            OccurredAt: DateTimeOffset.UtcNow), context.CancellationToken);

        logger.LogInformation("Payment processed for ticket {TicketId}, invoice {InvoiceId}",
            e.TicketId, invoiceId);
    }
}
