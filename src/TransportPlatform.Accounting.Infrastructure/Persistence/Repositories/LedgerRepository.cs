using Microsoft.EntityFrameworkCore;
using TransportPlatform.Accounting.Domain.Entities;
using TransportPlatform.Accounting.Domain.Enums;
using TransportPlatform.Accounting.Domain.Interfaces;

namespace TransportPlatform.Accounting.Infrastructure.Persistence.Repositories;

public class LedgerRepository(AccountingDbContext db) : ILedgerRepository
{
    public Task<LedgerEntry?> GetByTicketIdAsync(Guid ticketId, CancellationToken ct) =>
        db.LedgerEntries.FirstOrDefaultAsync(e => e.TicketId == ticketId, ct);

    public async Task<IEnumerable<LedgerEntry>> GetAllAsync(int page, int pageSize, CancellationToken ct) =>
        await db.LedgerEntries
            .OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task<decimal> GetCurrentBalanceAsync(CancellationToken ct)
    {
        var credits = await db.LedgerEntries
            .Where(e => e.Type == EntryType.Credit && e.Status == LedgerEntryStatus.Confirmed)
            .SumAsync(e => e.Amount, ct);

        var debits = await db.LedgerEntries
            .Where(e => e.Type == EntryType.Debit && e.Status == LedgerEntryStatus.Confirmed)
            .SumAsync(e => e.Amount, ct);

        return credits - debits;
    }

    public async Task AddAsync(LedgerEntry entry, CancellationToken ct)
    {
        await db.LedgerEntries.AddAsync(entry, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(LedgerEntry entry, CancellationToken ct)
    {
        db.LedgerEntries.Update(entry);
        await db.SaveChangesAsync(ct);
    }
}
