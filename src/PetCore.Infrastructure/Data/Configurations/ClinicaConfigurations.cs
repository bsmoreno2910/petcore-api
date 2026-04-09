using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCore.Domain.Entities;

namespace PetCore.Infrastructure.Data.Configurations;

public class ClinicaConfiguration : IEntityTypeConfiguration<Clinica>
{
    public void Configure(EntityTypeBuilder<Clinica> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Nome).IsRequired().HasMaxLength(200);
        builder.Property(c => c.NomeFantasia).HasMaxLength(200);
        builder.Property(c => c.RazaoSocial).HasMaxLength(300);
        builder.Property(c => c.Cnpj).HasMaxLength(18);
        builder.Property(c => c.Telefone).HasMaxLength(20);
        builder.Property(c => c.Email).HasMaxLength(200);
        builder.Property(c => c.Website).HasMaxLength(300);
        builder.Property(c => c.LogoUrl).HasMaxLength(500);
        builder.Property(c => c.Rua).HasMaxLength(300);
        builder.Property(c => c.Numero).HasMaxLength(20);
        builder.Property(c => c.Complemento).HasMaxLength(100);
        builder.Property(c => c.Bairro).HasMaxLength(100);
        builder.Property(c => c.Cidade).HasMaxLength(100);
        builder.Property(c => c.Estado).HasMaxLength(2);
        builder.Property(c => c.Cep).HasMaxLength(10);

        builder.HasIndex(c => c.Cnpj).IsUnique().HasFilter("\"cnpj\" IS NOT NULL");
    }
}

public class ClinicaUsuarioConfiguration : IEntityTypeConfiguration<ClinicaUsuario>
{
    public void Configure(EntityTypeBuilder<ClinicaUsuario> builder)
    {
        builder.HasKey(cu => cu.Id);
        builder.Property(cu => cu.Perfil).HasConversion<string>().HasMaxLength(50);

        builder.HasOne(cu => cu.Clinica).WithMany(c => c.ClinicaUsuarios)
            .HasForeignKey(cu => cu.ClinicaId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(cu => cu.Usuario).WithMany(u => u.ClinicaUsuarios)
            .HasForeignKey(cu => cu.UsuarioId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(cu => new { cu.ClinicaId, cu.UsuarioId }).IsUnique();
    }
}

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Nome).IsRequired().HasMaxLength(200);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(200);
        builder.Property(u => u.SenhaHash).IsRequired().HasMaxLength(200);
        builder.Property(u => u.Telefone).HasMaxLength(20);
        builder.Property(u => u.Crmv).HasMaxLength(20);
        builder.Property(u => u.AvatarUrl).HasMaxLength(500);

        builder.HasIndex(u => u.Email).IsUnique();
    }
}

public class TokenAtualizacaoConfiguration : IEntityTypeConfiguration<TokenAtualizacao>
{
    public void Configure(EntityTypeBuilder<TokenAtualizacao> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Token).IsRequired().HasMaxLength(200);
        builder.Property(t => t.SubstituidoPor).HasMaxLength(200);

        builder.HasOne(t => t.Usuario).WithMany()
            .HasForeignKey(t => t.UsuarioId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => t.Token).IsUnique();
    }
}
