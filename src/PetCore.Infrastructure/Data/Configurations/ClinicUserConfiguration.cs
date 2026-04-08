using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCore.Domain.Entities;

namespace PetCore.Infrastructure.Data.Configurations;

public class ClinicUserConfiguration : IEntityTypeConfiguration<ClinicUser>
{
    public void Configure(EntityTypeBuilder<ClinicUser> builder)
    {
        builder.HasKey(cu => cu.Id);
        builder.Property(cu => cu.Role).HasConversion<string>().HasMaxLength(50);

        builder.HasOne(cu => cu.Clinic)
            .WithMany(c => c.ClinicUsers)
            .HasForeignKey(cu => cu.ClinicId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cu => cu.User)
            .WithMany(u => u.ClinicUsers)
            .HasForeignKey(cu => cu.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(cu => new { cu.ClinicId, cu.UserId }).IsUnique();
    }
}
