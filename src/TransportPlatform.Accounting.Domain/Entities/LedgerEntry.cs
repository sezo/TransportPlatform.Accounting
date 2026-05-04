using TransportPlatform.Accounting.Domain.Enums;
using TransportPlatform.Accounting.Domain.Exceptions;

namespace TransportPlatform.Accounting.Domain.Entities;

public class LedgerEntry
{
    public Guid Id { get; private set; }
    public Guid TicketId { get; private set; }
    public Guid InvoiceId { get; private set; }
    public decimal Amount { get; private set; }
    public EntryType Type { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public LedgerEntryStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? ConfirmedAt { get; private set; }

    private LedgerEntry() { }

    public static LedgerEntry CreatePending(
        Guid ticketId,
        Guid invoiceId,
        decimal amount,
        EntryType type,
        string description)
    {
        if (amount <= 0)
            throw new AccountingDomainException("Ledger entry amount must be positive.");
        if (string.IsNullOrWhiteSpace(description))
            throw new AccountingDomainException("Ledger entry description is required.");

        return new LedgerEntry
        {
            Id = Guid.NewGuid(),
            TicketId = ticketId,
            InvoiceId = invoiceId,
            Amount = amount,
            Type = type,
            Description = description,
            Status = LedgerEntryStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public void Confirm()
    {
        if (Status != LedgerEntryStatus.Pending)
            throw new AccountingDomainException($"Only pending entries can be confirmed. Current status: {Status}.");

        Status = LedgerEntryStatus.Confirmed;
        ConfirmedAt = DateTimeOffset.UtcNow;
    }

    public void Reverse()
    {
        if (Status == LedgerEntryStatus.Reversed)
            throw new AccountingDomainException("Ledger entry is already reversed.");

        Status = LedgerEntryStatus.Reversed;
    }
}
