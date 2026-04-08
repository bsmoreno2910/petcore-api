using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCore.Domain.Entities;

namespace PetCore.Infrastructure.Data.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Sex).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.Weight).HasPrecision(8, 2);
        builder.Property(p => p.Color).HasMaxLength(100);
        builder.Property(p => p.Microchip).HasMaxLength(50);

        builder.HasOne(p => p.Clinic)
            .WithMany(c => c.Patients)
            .HasForeignKey(p => p.ClinicId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Tutor)
            .WithMany(t => t.Patients)
            .HasForeignKey(p => p.TutorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Species)
            .WithMany()
            .HasForeignKey(p => p.SpeciesId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Breed)
            .WithMany()
            .HasForeignKey(p => p.BreedId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
