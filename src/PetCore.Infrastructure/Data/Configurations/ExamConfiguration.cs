using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCore.Domain.Entities;

namespace PetCore.Infrastructure.Data.Configurations;

public class ExamTypeConfiguration : IEntityTypeConfiguration<ExamType>
{
    public void Configure(EntityTypeBuilder<ExamType> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Category).HasMaxLength(100);
        builder.Property(e => e.DefaultPrice).HasPrecision(10, 2);
    }
}

public class ExamRequestConfiguration : IEntityTypeConfiguration<ExamRequest>
{
    public void Configure(EntityTypeBuilder<ExamRequest> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(30);

        builder.HasOne(e => e.Patient)
            .WithMany(p => p.ExamRequests)
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.RequestedBy)
            .WithMany()
            .HasForeignKey(e => e.RequestedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ExamType)
            .WithMany()
            .HasForeignKey(e => e.ExamTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.MedicalRecord)
            .WithMany(m => m.ExamRequests)
            .HasForeignKey(e => e.MedicalRecordId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class ExamResultConfiguration : IEntityTypeConfiguration<ExamResult>
{
    public void Configure(EntityTypeBuilder<ExamResult> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasOne(e => e.ExamRequest)
            .WithOne(r => r.Result)
            .HasForeignKey<ExamResult>(e => e.ExamRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.PerformedBy)
            .WithMany()
            .HasForeignKey(e => e.PerformedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
