using Microsoft.EntityFrameworkCore;
using TransportPlatform.Accounting.Domain.Entities;

namespace TransportPlatform.Accounting.Infrastructure.Persistence;

public class AccountingDbContext(DbContextOptions<AccountingDbContext> options) : DbContext(options)
{
    public DbSet<LedgerEntry> LedgerEntries => Set<LedgerEntry>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Employee> Employees => Set<Employee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccountingDbContext).Assembly);
    }
}
