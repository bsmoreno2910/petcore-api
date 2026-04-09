using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCore.Domain.Entities;

namespace PetCore.Infrastructure.Data.Configurations;

public class TipoExameConfiguration : IEntityTypeConfiguration<TipoExame>
{
    public void Configure(EntityTypeBuilder<TipoExame> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Nome).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Categoria).HasMaxLength(100);
        builder.Property(e => e.PrecoDefault).HasPrecision(10, 2);
    }
}

public class SolicitacaoExameConfiguration : IEntityTypeConfiguration<SolicitacaoExame>
{
    public void Configure(EntityTypeBuilder<SolicitacaoExame> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(30);

        builder.HasOne(e => e.Paciente).WithMany(p => p.SolicitacoesExame)
            .HasForeignKey(e => e.PacienteId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.SolicitadoPor).WithMany()
            .HasForeignKey(e => e.SolicitadoPorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.TipoExame).WithMany()
            .HasForeignKey(e => e.TipoExameId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Prontuario).WithMany(m => m.SolicitacoesExame)
            .HasForeignKey(e => e.ProntuarioId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class ResultadoExameConfiguration : IEntityTypeConfiguration<ResultadoExame>
{
    public void Configure(EntityTypeBuilder<ResultadoExame> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasOne(e => e.SolicitacaoExame).WithOne(r => r.Resultado)
            .HasForeignKey<ResultadoExame>(e => e.SolicitacaoExameId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.RealizadoPor).WithMany()
            .HasForeignKey(e => e.RealizadoPorId).OnDelete(DeleteBehavior.Restrict);
    }
}
