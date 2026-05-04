using TransportPlatform.Accounting.Application.Queries;
using TransportPlatform.Accounting.Domain.Enums;
using TransportPlatform.Accounting.Domain.Interfaces;

namespace TransportPlatform.Accounting.Application.Handlers;

public class GetLedgerHandler(ILedgerRepository ledger)
{
    public async Task<LedgerSummaryDto> HandleSummaryAsync(
        GetLedgerSummaryQuery _,
        CancellationToken ct = default)
    {
        var balance = await ledger.GetCurrentBalanceAsync(ct);
        var entries = await ledger.GetAllAsync(1, int.MaxValue, ct);
        var list = entries.ToList();

        return new LedgerSummaryDto(
            CurrentBalance: balance,
            TotalEntries: list.Count,
            TotalCredits: list.Where(e => e.Type == EntryType.Credit).Sum(e => e.Amount),
            TotalDebits: list.Where(e => e.Type == EntryType.Debit).Sum(e => e.Amount),
            AsOf: DateTimeOffset.UtcNow);
    }

    public async Task<PagedResult<LedgerEntryDto>> HandleEntriesAsync(
        GetLedgerEntriesQuery query,
        CancellationToken ct = default)
    {
        var entries = await ledger.GetAllAsync(query.Page, query.PageSize, ct);
        var balance = await ledger.GetCurrentBalanceAsync(ct);
        var dtos = entries.Select(e => new LedgerEntryDto(
            e.Id, e.TicketId, e.InvoiceId, e.Amount,
            e.Type.ToString(), e.Description, e.Status.ToString(),
            e.CreatedAt, e.ConfirmedAt));

        return new PagedResult<LedgerEntryDto>(dtos, query.Page, query.PageSize, (int)balance);
    }
}
