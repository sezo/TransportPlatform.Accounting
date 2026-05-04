using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransportPlatform.Accounting.Domain.Entities;

namespace TransportPlatform.Accounting.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.IdentityId);
        builder.Property(c => c.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(c => c.LastName).HasMaxLength(100).IsRequired();
        builder.Property(c => c.Email).HasMaxLength(256).IsRequired();
        builder.HasIndex(c => c.Email).IsUnique();
        builder.HasIndex(c => c.IdentityId).IsUnique().HasFilter("\"IdentityId\" IS NOT NULL");
    }
}
