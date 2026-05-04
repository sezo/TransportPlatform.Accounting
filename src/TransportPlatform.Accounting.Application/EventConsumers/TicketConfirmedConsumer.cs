using MassTransit;
using Microsoft.Extensions.Logging;
using TransportPlatform.Contracts.Events.Accounting;
using TransportPlatform.Contracts.Events.Ticketing;
using TransportPlatform.Contracts.Messaging;

namespace TransportPlatform.Accounting.Application.EventConsumers;

/// <summary>
/// Responds to TicketConfirmed (saga completed successfully):
///   Publishes InvoiceFiscalized to signal the invoice is ready for fiscal compliance.
///   In production this would call a real fiscal API.
/// </summary>
public class TicketConfirmedConsumer(
    IEventPublisher publisher,
    ILogger<TicketConfirmedConsumer> logger) : IConsumer<TicketConfirmed>
{
    public async Task Consume(ConsumeContext<TicketConfirmed> context)
    {
        var e = context.Message;
        var fiscalNumber = $"FISCAL-{DateTime.UtcNow:yyyyMMdd}-{e.TicketId.ToString()[..8].ToUpper()}";

        logger.LogInformation("Fiscalizing invoice for confirmed ticket {TicketId}, fiscal# {FiscalNumber}",
            e.TicketId, fiscalNumber);

        await publisher.PublishAsync(new InvoiceFiscalized(
            TicketId: e.TicketId,
            InvoiceId: Guid.NewGuid(),
            FiscalNumber: fiscalNumber,
            OccurredAt: DateTimeOffset.UtcNow), context.CancellationToken);
    }
}
