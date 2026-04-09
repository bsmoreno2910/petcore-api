using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCore.Domain.Entities;

namespace PetCore.Infrastructure.Data.Configurations;

public class EspecieConfiguration : IEntityTypeConfiguration<Especie>
{
    public void Configure(EntityTypeBuilder<Especie> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Nome).IsRequired().HasMaxLength(100);
        builder.HasIndex(s => s.Nome).IsUnique();
    }
}

public class RacaConfiguration : IEntityTypeConfiguration<Raca>
{
    public void Configure(EntityTypeBuilder<Raca> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Nome).IsRequired().HasMaxLength(100);
        builder.HasOne(b => b.Especie).WithMany(s => s.Racas)
            .HasForeignKey(b => b.EspecieId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class TutorConfiguration : IEntityTypeConfiguration<Tutor>
{
    public void Configure(EntityTypeBuilder<Tutor> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Nome).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Cpf).HasMaxLength(14);
        builder.Property(t => t.Rg).HasMaxLength(20);
        builder.Property(t => t.Telefone).HasMaxLength(20);
        builder.Property(t => t.TelefoneSecundario).HasMaxLength(20);
        builder.Property(t => t.Email).HasMaxLength(200);
        builder.Property(t => t.Rua).HasMaxLength(300);
        builder.Property(t => t.Numero).HasMaxLength(20);
        builder.Property(t => t.Complemento).HasMaxLength(100);
        builder.Property(t => t.Bairro).HasMaxLength(100);
        builder.Property(t => t.Cidade).HasMaxLength(100);
        builder.Property(t => t.Estado).HasMaxLength(2);
        builder.Property(t => t.Cep).HasMaxLength(10);

        builder.HasOne(t => t.Clinica).WithMany()
            .HasForeignKey(t => t.ClinicaId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(t => new { t.ClinicaId, t.Cpf }).IsUnique().HasFilter("\"cpf\" IS NOT NULL");
        builder.HasIndex(t => new { t.ClinicaId, t.Email }).IsUnique().HasFilter("\"email\" IS NOT NULL");
    }
}

public class PacienteConfiguration : IEntityTypeConfiguration<Paciente>
{
    public void Configure(EntityTypeBuilder<Paciente> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Nome).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Sexo).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.Peso).HasPrecision(8, 2);
        builder.Property(p => p.Cor).HasMaxLength(100);
        builder.Property(p => p.Microchip).HasMaxLength(50);

        builder.HasOne(p => p.Clinica).WithMany(c => c.Pacientes)
            .HasForeignKey(p => p.ClinicaId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.Tutor).WithMany(t => t.Pacientes)
            .HasForeignKey(p => p.TutorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.Especie).WithMany()
            .HasForeignKey(p => p.EspecieId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.Raca).WithMany()
            .HasForeignKey(p => p.RacaId).OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(p => new { p.ClinicaId, p.Nome });
    }
}
