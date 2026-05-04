using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransportPlatform.Accounting.Domain.Entities;

namespace TransportPlatform.Accounting.Infrastructure.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("employees");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IdentityId);
        builder.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.LastName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Email).HasMaxLength(256).IsRequired();
        builder.Property(e => e.Position).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Department).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Salary).HasPrecision(18, 2);
        builder.HasIndex(e => e.Department);
        builder.HasIndex(e => e.Email).IsUnique();
        builder.HasIndex(e => e.IdentityId).IsUnique().HasFilter("\"IdentityId\" IS NOT NULL");
    }
}
