using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCore.Domain.Entities;

namespace PetCore.Infrastructure.Data.Configurations;

public class MedicalRecordConfiguration : IEntityTypeConfiguration<MedicalRecord>
{
    public void Configure(EntityTypeBuilder<MedicalRecord> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Weight).HasPrecision(8, 2);
        builder.Property(m => m.Temperature).HasPrecision(5, 2);

        builder.HasOne(m => m.Patient)
            .WithMany(p => p.MedicalRecords)
            .HasForeignKey(m => m.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Veterinarian)
            .WithMany(u => u.MedicalRecords)
            .HasForeignKey(m => m.VeterinarianId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Appointment)
            .WithOne(a => a.MedicalRecord)
            .HasForeignKey<MedicalRecord>(m => m.AppointmentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
{
    public void Configure(EntityTypeBuilder<Prescription> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.MedicineName).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Dosage).HasMaxLength(100);
        builder.Property(p => p.Frequency).HasMaxLength(100);
        builder.Property(p => p.Duration).HasMaxLength(100);
        builder.Property(p => p.Route).HasMaxLength(50);

        builder.HasOne(p => p.MedicalRecord)
            .WithMany(m => m.Prescriptions)
            .HasForeignKey(p => p.MedicalRecordId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
