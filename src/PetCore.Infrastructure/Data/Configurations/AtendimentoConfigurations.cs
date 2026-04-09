using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCore.Domain.Entities;

namespace PetCore.Infrastructure.Data.Configurations;

public class AgendamentoConfiguration : IEntityTypeConfiguration<Agendamento>
{
    public void Configure(EntityTypeBuilder<Agendamento> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Tipo).HasConversion<string>().HasMaxLength(30);
        builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(a => a.Motivo).HasMaxLength(500);

        builder.HasOne(a => a.Clinica).WithMany(c => c.Agendamentos)
            .HasForeignKey(a => a.ClinicaId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(a => a.Paciente).WithMany(p => p.Agendamentos)
            .HasForeignKey(a => a.PacienteId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(a => a.Veterinario).WithMany(u => u.AgendamentosComoVet)
            .HasForeignKey(a => a.VeterinarioId).OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(a => new { a.ClinicaId, a.DataHoraAgendada });
    }
}

public class ProntuarioConfiguration : IEntityTypeConfiguration<Prontuario>
{
    public void Configure(EntityTypeBuilder<Prontuario> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Peso).HasPrecision(8, 2);
        builder.Property(m => m.Temperatura).HasPrecision(5, 2);

        builder.HasOne(m => m.Paciente).WithMany(p => p.Prontuarios)
            .HasForeignKey(m => m.PacienteId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(m => m.Veterinario).WithMany(u => u.Prontuarios)
            .HasForeignKey(m => m.VeterinarioId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(m => m.Agendamento).WithOne(a => a.Prontuario)
            .HasForeignKey<Prontuario>(m => m.AgendamentoId).OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(m => new { m.ClinicaId, m.CriadoEm });
    }
}

public class PrescricaoConfiguration : IEntityTypeConfiguration<Prescricao>
{
    public void Configure(EntityTypeBuilder<Prescricao> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.NomeMedicamento).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Dosagem).HasMaxLength(100);
        builder.Property(p => p.Frequencia).HasMaxLength(100);
        builder.Property(p => p.Duracao).HasMaxLength(100);
        builder.Property(p => p.ViaAdministracao).HasMaxLength(50);

        builder.HasOne(p => p.Prontuario).WithMany(m => m.Prescricoes)
            .HasForeignKey(p => p.ProntuarioId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class InternacaoConfiguration : IEntityTypeConfiguration<Internacao>
{
    public void Configure(EntityTypeBuilder<Internacao> builder)
    {
        builder.HasKey(h => h.Id);
        builder.Property(h => h.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(h => h.Baia).HasMaxLength(50);

        builder.HasOne(h => h.Paciente).WithMany(p => p.Internacoes)
            .HasForeignKey(h => h.PacienteId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(h => h.Veterinario).WithMany()
            .HasForeignKey(h => h.VeterinarioId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class EvolucaoConfiguration : IEntityTypeConfiguration<Evolucao>
{
    public void Configure(EntityTypeBuilder<Evolucao> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Peso).HasPrecision(8, 2);
        builder.Property(e => e.Temperatura).HasPrecision(5, 2);

        builder.HasOne(e => e.Internacao).WithMany(h => h.Evolucoes)
            .HasForeignKey(e => e.InternacaoId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Veterinario).WithMany()
            .HasForeignKey(e => e.VeterinarioId).OnDelete(DeleteBehavior.Restrict);
    }
}
