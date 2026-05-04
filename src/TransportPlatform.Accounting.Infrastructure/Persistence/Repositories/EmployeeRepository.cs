using Microsoft.EntityFrameworkCore;
using TransportPlatform.Accounting.Domain.Entities;
using TransportPlatform.Accounting.Domain.Interfaces;

namespace TransportPlatform.Accounting.Infrastructure.Persistence.Repositories;

public class EmployeeRepository(AccountingDbContext db) : IEmployeeRepository
{
    public Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct) =>
        db.Employees.FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IEnumerable<Employee>> GetAllAsync(int page, int pageSize, CancellationToken ct) =>
        await db.Employees
            .OrderBy(e => e.Department).ThenBy(e => e.LastName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task AddAsync(Employee employee, CancellationToken ct)
    {
        await db.Employees.AddAsync(employee, ct);
        await db.SaveChangesAsync(ct);
    }
}
