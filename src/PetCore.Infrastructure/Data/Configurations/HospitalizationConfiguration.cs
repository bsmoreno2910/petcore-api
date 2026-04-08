using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCore.Domain.Entities;

namespace PetCore.Infrastructure.Data.Configurations;

public class HospitalizationConfiguration : IEntityTypeConfiguration<Hospitalization>
{
    public void Configure(EntityTypeBuilder<Hospitalization> builder)
    {
        builder.HasKey(h => h.Id);
        builder.Property(h => h.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(h => h.Cage).HasMaxLength(50);

        builder.HasOne(h => h.Patient)
            .WithMany(p => p.Hospitalizations)
            .HasForeignKey(h => h.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(h => h.Veterinarian)
            .WithMany()
            .HasForeignKey(h => h.VeterinarianId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class HospitalizationEvolutionConfiguration : IEntityTypeConfiguration<HospitalizationEvolution>
{
    public void Configure(EntityTypeBuilder<HospitalizationEvolution> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Weight).HasPrecision(8, 2);
        builder.Property(e => e.Temperature).HasPrecision(5, 2);

        builder.HasOne(e => e.Hospitalization)
            .WithMany(h => h.Evolutions)
            .HasForeignKey(e => e.HospitalizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Veterinarian)
            .WithMany()
            .HasForeignKey(e => e.VeterinarianId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
