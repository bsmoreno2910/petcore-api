using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCore.Domain.Entities;

namespace PetCore.Infrastructure.Data.Configurations;

public class ClinicConfiguration : IEntityTypeConfiguration<Clinic>
{
    public void Configure(EntityTypeBuilder<Clinic> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.TradeName).HasMaxLength(200);
        builder.Property(c => c.LegalName).HasMaxLength(300);
        builder.Property(c => c.Cnpj).HasMaxLength(18);
        builder.Property(c => c.Phone).HasMaxLength(20);
        builder.Property(c => c.Email).HasMaxLength(200);
        builder.Property(c => c.Website).HasMaxLength(300);
        builder.Property(c => c.LogoUrl).HasMaxLength(500);
        builder.Property(c => c.Street).HasMaxLength(300);
        builder.Property(c => c.Number).HasMaxLength(20);
        builder.Property(c => c.Complement).HasMaxLength(100);
        builder.Property(c => c.Neighborhood).HasMaxLength(100);
        builder.Property(c => c.City).HasMaxLength(100);
        builder.Property(c => c.State).HasMaxLength(2);
        builder.Property(c => c.ZipCode).HasMaxLength(10);

        builder.HasIndex(c => c.Cnpj).IsUnique().HasFilter("\"Cnpj\" IS NOT NULL");
    }
}
