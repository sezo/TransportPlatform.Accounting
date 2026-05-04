namespace TransportPlatform.Accounting.Application.Queries;

public record GetLedgerSummaryQuery;

public record LedgerSummaryDto(
    decimal CurrentBalance,
    int TotalEntries,
    decimal TotalCredits,
    decimal TotalDebits,
    DateTimeOffset AsOf);

public record GetLedgerEntriesQuery(int Page = 1, int PageSize = 20);

public record LedgerEntryDto(
    Guid Id,
    Guid TicketId,
    Guid InvoiceId,
    decimal Amount,
    string Type,
    string Description,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ConfirmedAt);

public record PagedResult<T>(IEnumerable<T> Items, int Page, int PageSize, int TotalCount);
