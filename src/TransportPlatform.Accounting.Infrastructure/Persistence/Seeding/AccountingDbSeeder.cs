using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TransportPlatform.Accounting.Domain.Entities;

namespace TransportPlatform.Accounting.Infrastructure.Persistence.Seeding;

public class AccountingDbSeeder(AccountingDbContext db, ILogger<AccountingDbSeeder> logger)
{
    public async Task SeedAsync(CancellationToken ct = default)
    {
        if (await db.Employees.AnyAsync(ct))
            return;

        var employees = new[]
        {
            Employee.Create("Ana",     "Kovač",     new DateOnly(1985, 3, 15), "ana.kovac@transport.hr",      "CFO",               "Finance", new DateOnly(2018, 1, 10), 5200m),
            Employee.Create("Marko",   "Horvat",    new DateOnly(1990, 7, 22), "marko.horvat@transport.hr",   "Accountant",        "Finance", new DateOnly(2020, 6, 1),  3100m),
            Employee.Create("Ivana",   "Perić",     new DateOnly(1988, 11, 5), "ivana.peric@transport.hr",    "Senior Accountant", "Finance", new DateOnly(2019, 3, 15), 3800m),
            Employee.Create("Tomislav","Blažević",  new DateOnly(1992, 4, 8),  "tomislav.blazevic@transport.hr","Payroll Specialist","HR",    new DateOnly(2021, 9, 1),  2900m),
            Employee.Create("Maja",    "Šimić",     new DateOnly(1995, 1, 30), "maja.simic@transport.hr",     "Finance Analyst",   "Finance", new DateOnly(2022, 2, 14), 2700m),
        };

        await db.Employees.AddRangeAsync(employees, ct);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Seeded {Count} demo employees", employees.Length);
    }
}
