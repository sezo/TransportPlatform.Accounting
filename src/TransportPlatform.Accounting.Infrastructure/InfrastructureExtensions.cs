using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;
using TransportPlatform.Accounting.Application.EventConsumers;
using TransportPlatform.Accounting.Application.Interfaces;
using TransportPlatform.Accounting.Domain.Interfaces;
using TransportPlatform.Accounting.Infrastructure.Identity;
using TransportPlatform.Accounting.Infrastructure.Persistence;
using TransportPlatform.Accounting.Infrastructure.Persistence.Repositories;
using TransportPlatform.Accounting.Infrastructure.Persistence.Seeding;
using TransportPlatform.Infrastructure.Common.Messaging;

namespace TransportPlatform.Accounting.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddAccountingInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        // ── Database ──────────────────────────────────────────────────────────
        services.AddDbContext<AccountingDbContext>(options =>
            options.UseNpgsql(
                config.GetConnectionString("AccountingDb"),
                npgsql => npgsql.MigrationsAssembly(
                    typeof(AccountingDbContext).Assembly.GetName().Name)));

        // ── Repositories ──────────────────────────────────────────────────────
        services.AddScoped<ILedgerRepository, LedgerRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        // ── Messaging: publish + consume Ticketing events ─────────────────────
        services.AddTransportMessaging(config, x =>
        {
            x.AddConsumer<TicketReservedConsumer>();
            x.AddConsumer<TicketCancelledConsumer>();
            x.AddConsumer<TicketConfirmedConsumer>();
        });

        // ── Database tracing ──────────────────────────────────────────────────
        services.AddOpenTelemetry()
            .WithTracing(tracing => tracing.AddSource("Npgsql"));

        // ── Identity (Keycloak Admin API) ─────────────────────────────────────
        services.AddHttpClient("keycloak-admin");
        services.AddScoped<IIdentityService, KeycloakIdentityService>();

        // ── Seeder ────────────────────────────────────────────────────────────
        services.AddScoped<AccountingDbSeeder>();

        return services;
    }

    public static async Task InitialiseDatabaseAsync(
        this IServiceProvider services,
        CancellationToken ct = default)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AccountingDbContext>();
        await db.Database.MigrateAsync(ct);

        var seeder = scope.ServiceProvider.GetRequiredService<AccountingDbSeeder>();
        await seeder.SeedAsync(ct);
    }
}
