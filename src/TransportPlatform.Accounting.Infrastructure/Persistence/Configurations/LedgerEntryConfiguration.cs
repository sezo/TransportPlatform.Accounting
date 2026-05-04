using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransportPlatform.Accounting.Domain.Entities;

namespace TransportPlatform.Accounting.Infrastructure.Persistence.Configurations;

public class LedgerEntryConfiguration : IEntityTypeConfiguration<LedgerEntry>
{
    public void Configure(EntityTypeBuilder<LedgerEntry> builder)
    {
        builder.ToTable("ledger_entries");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Amount).HasPrecision(18, 2);
        builder.Property(e => e.Type).HasConversion<string>().HasMaxLength(10);
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.HasIndex(e => e.TicketId);
        builder.HasIndex(e => e.InvoiceId);
        builder.HasIndex(e => e.Status);
    }
}
