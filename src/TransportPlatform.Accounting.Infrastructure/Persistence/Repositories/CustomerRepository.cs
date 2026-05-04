using Microsoft.EntityFrameworkCore;
using TransportPlatform.Accounting.Domain.Entities;
using TransportPlatform.Accounting.Domain.Interfaces;

namespace TransportPlatform.Accounting.Infrastructure.Persistence.Repositories;

public class CustomerRepository(AccountingDbContext db) : ICustomerRepository
{
    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct) =>
        db.Customers.FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<Customer?> GetByEmailAsync(string email, CancellationToken ct) =>
        db.Customers.FirstOrDefaultAsync(c => c.Email == email.ToLowerInvariant(), ct);

    public Task<Customer?> GetByIdentityIdAsync(Guid identityId, CancellationToken ct) =>
        db.Customers.FirstOrDefaultAsync(c => c.IdentityId == identityId, ct);

    public async Task<IEnumerable<Customer>> GetAllAsync(int page, int pageSize, CancellationToken ct) =>
        await db.Customers
            .OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task AddAsync(Customer customer, CancellationToken ct)
    {
        await db.Customers.AddAsync(customer, ct);
        await db.SaveChangesAsync(ct);
    }
}
