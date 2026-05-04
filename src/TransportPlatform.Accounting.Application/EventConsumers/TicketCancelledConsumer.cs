using MassTransit;
using Microsoft.Extensions.Logging;
using TransportPlatform.Accounting.Domain.Interfaces;
using TransportPlatform.Contracts.Events.Accounting;
using TransportPlatform.Contracts.Events.Ticketing;
using TransportPlatform.Contracts.Messaging;

namespace TransportPlatform.Accounting.Application.EventConsumers;

/// <summary>
/// Responds to TicketCancelled:
///   1. Reverses the ledger entry (debit to offset the original credit)
///   2. Publishes PaymentRefunded for reporting / external systems
/// </summary>
public class TicketCancelledConsumer(
    ILedgerRepository ledger,
    IEventPublisher publisher,
    ILogger<TicketCancelledConsumer> logger) : IConsumer<TicketCancelled>
{
    public async Task Consume(ConsumeContext<TicketCancelled> context)
    {
        var e = context.Message;
        logger.LogInformation("Processing refund for cancelled ticket {TicketId}", e.TicketId);

        var entry = await ledger.GetByTicketIdAsync(e.TicketId, context.CancellationToken);
        if (entry is null)
        {
            logger.LogWarning("No ledger entry found for ticket {TicketId} — skipping refund", e.TicketId);
            return;
        }

        entry.Reverse();
        await ledger.UpdateAsync(entry, context.CancellationToken);

        await publisher.PublishAsync(new PaymentRefunded(
            TicketId: e.TicketId,
            InvoiceId: entry.InvoiceId,
            Amount: entry.Amount,
            OccurredAt: DateTimeOffset.UtcNow), context.CancellationToken);

        logger.LogInformation("Refund completed for ticket {TicketId}, amount {Amount}",
            e.TicketId, entry.Amount);
    }
}
