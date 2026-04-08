using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCore.Domain.Entities;

namespace PetCore.Infrastructure.Data.Configurations;

public class FinancialCategoryConfiguration : IEntityTypeConfiguration<FinancialCategory>
{
    public void Configure(EntityTypeBuilder<FinancialCategory> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Type).HasConversion<string>().HasMaxLength(20);
    }
}

public class FinancialTransactionConfiguration : IEntityTypeConfiguration<FinancialTransaction>
{
    public void Configure(EntityTypeBuilder<FinancialTransaction> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.Description).IsRequired().HasMaxLength(500);
        builder.Property(t => t.Amount).HasPrecision(14, 2);
        builder.Property(t => t.Discount).HasPrecision(14, 2);
        builder.Property(t => t.AmountPaid).HasPrecision(14, 2);
        builder.Property(t => t.PaymentMethod).HasConversion<string>().HasMaxLength(30);
        builder.Property(t => t.InvoiceNumber).HasMaxLength(50);

        builder.HasOne(t => t.Clinic)
            .WithMany(c => c.Transactions)
            .HasForeignKey(t => t.ClinicId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.FinancialCategory)
            .WithMany()
            .HasForeignKey(t => t.FinancialCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Tutor)
            .WithMany(tu => tu.Transactions)
            .HasForeignKey(t => t.TutorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.Appointment)
            .WithMany(a => a.Transactions)
            .HasForeignKey(t => t.AppointmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.Hospitalization)
            .WithMany(h => h.Transactions)
            .HasForeignKey(t => t.HospitalizationId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.ExamRequest)
            .WithMany(e => e.Transactions)
            .HasForeignKey(t => t.ExamRequestId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.CostCenter)
            .WithMany(cc => cc.Transactions)
            .HasForeignKey(t => t.CostCenterId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.CreatedBy)
            .WithMany()
            .HasForeignKey(t => t.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class TransactionInstallmentConfiguration : IEntityTypeConfiguration<TransactionInstallment>
{
    public void Configure(EntityTypeBuilder<TransactionInstallment> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Amount).HasPrecision(14, 2);
        builder.Property(i => i.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasOne(i => i.Transaction)
            .WithMany(t => t.Installments)
            .HasForeignKey(i => i.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
