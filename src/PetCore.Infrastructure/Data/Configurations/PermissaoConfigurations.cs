using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCore.Domain.Entities;

namespace PetCore.Infrastructure.Data.Configurations;

public class PermissaoConfiguration : IEntityTypeConfiguration<Permissao>
{
    public void Configure(EntityTypeBuilder<Permissao> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Perfil).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Modulo).IsRequired().HasMaxLength(50);

        builder.HasIndex(p => new { p.Perfil, p.Modulo }).IsUnique();
    }
}
