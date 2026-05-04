using TransportPlatform.Accounting.Domain.Entities;

namespace TransportPlatform.Accounting.Domain.Interfaces;

public interface ILedgerRepository
{
    Task<LedgerEntry?> GetByTicketIdAsync(Guid ticketId, CancellationToken ct = default);
    Task<IEnumerable<LedgerEntry>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    Task<decimal> GetCurrentBalanceAsync(CancellationToken ct = default);
    Task AddAsync(LedgerEntry entry, CancellationToken ct = default);
    Task UpdateAsync(LedgerEntry entry, CancellationToken ct = default);
}
