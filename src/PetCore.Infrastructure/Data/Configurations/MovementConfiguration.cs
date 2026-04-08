using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCore.Domain.Entities;

namespace PetCore.Infrastructure.Data.Configurations;

public class MovementConfiguration : IEntityTypeConfiguration<Movement>
{
    public void Configure(EntityTypeBuilder<Movement> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(m => m.Reason).HasMaxLength(300);

        builder.HasOne(m => m.Product)
            .WithMany(p => p.Movements)
            .HasForeignKey(m => m.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.CreatedBy)
            .WithMany(u => u.MovementsCreated)
            .HasForeignKey(m => m.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.ApprovedBy)
            .WithMany(u => u.MovementsApproved)
            .HasForeignKey(m => m.ApprovedById)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(m => m.Order)
            .WithMany(o => o.Movements)
            .HasForeignKey(m => m.OrderId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
