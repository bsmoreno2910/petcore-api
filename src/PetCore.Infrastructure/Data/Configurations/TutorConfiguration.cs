using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCore.Domain.Entities;

namespace PetCore.Infrastructure.Data.Configurations;

public class TutorConfiguration : IEntityTypeConfiguration<Tutor>
{
    public void Configure(EntityTypeBuilder<Tutor> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Cpf).HasMaxLength(14);
        builder.Property(t => t.Rg).HasMaxLength(20);
        builder.Property(t => t.Phone).HasMaxLength(20);
        builder.Property(t => t.PhoneSecondary).HasMaxLength(20);
        builder.Property(t => t.Email).HasMaxLength(200);
        builder.Property(t => t.Street).HasMaxLength(300);
        builder.Property(t => t.Number).HasMaxLength(20);
        builder.Property(t => t.Complement).HasMaxLength(100);
        builder.Property(t => t.Neighborhood).HasMaxLength(100);
        builder.Property(t => t.City).HasMaxLength(100);
        builder.Property(t => t.State).HasMaxLength(2);
        builder.Property(t => t.ZipCode).HasMaxLength(10);

        builder.HasOne(t => t.Clinic)
            .WithMany()
            .HasForeignKey(t => t.ClinicId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => new { t.ClinicId, t.Cpf }).IsUnique().HasFilter("\"Cpf\" IS NOT NULL");
    }
}
