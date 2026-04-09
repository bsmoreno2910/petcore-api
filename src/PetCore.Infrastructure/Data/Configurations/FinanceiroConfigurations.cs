using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCore.Domain.Entities;

namespace PetCore.Infrastructure.Data.Configurations;

public class CategoriaFinanceiraConfiguration : IEntityTypeConfiguration<CategoriaFinanceira>
{
    public void Configure(EntityTypeBuilder<CategoriaFinanceira> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Nome).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Tipo).HasConversion<string>().HasMaxLength(20);
    }
}

public class TransacaoFinanceiraConfiguration : IEntityTypeConfiguration<TransacaoFinanceira>
{
    public void Configure(EntityTypeBuilder<TransacaoFinanceira> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Tipo).HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.Descricao).IsRequired().HasMaxLength(500);
        builder.Property(t => t.Valor).HasPrecision(14, 2);
        builder.Property(t => t.Desconto).HasPrecision(14, 2);
        builder.Property(t => t.ValorPago).HasPrecision(14, 2);
        builder.Property(t => t.MetodoPagamento).HasConversion<string>().HasMaxLength(30);
        builder.Property(t => t.NumeroNota).HasMaxLength(50);

        builder.HasOne(t => t.Clinica).WithMany(c => c.Transacoes)
            .HasForeignKey(t => t.ClinicaId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.CategoriaFinanceira).WithMany()
            .HasForeignKey(t => t.CategoriaFinanceiraId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Tutor).WithMany(tu => tu.Transacoes)
            .HasForeignKey(t => t.TutorId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(t => t.Agendamento).WithMany(a => a.Transacoes)
            .HasForeignKey(t => t.AgendamentoId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(t => t.Internacao).WithMany(h => h.Transacoes)
            .HasForeignKey(t => t.InternacaoId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(t => t.SolicitacaoExame).WithMany(e => e.Transacoes)
            .HasForeignKey(t => t.SolicitacaoExameId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(t => t.CentroCusto).WithMany(cc => cc.Transacoes)
            .HasForeignKey(t => t.CentroCustoId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(t => t.CriadoPor).WithMany()
            .HasForeignKey(t => t.CriadoPorId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => new { t.ClinicaId, t.DataVencimento });
        builder.HasIndex(t => new { t.ClinicaId, t.Status });
    }
}

public class ParcelaTransacaoConfiguration : IEntityTypeConfiguration<ParcelaTransacao>
{
    public void Configure(EntityTypeBuilder<ParcelaTransacao> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Valor).HasPrecision(14, 2);
        builder.Property(i => i.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasOne(i => i.Transacao).WithMany(t => t.Parcelas)
            .HasForeignKey(i => i.TransacaoId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class CentroCustoConfiguration : IEntityTypeConfiguration<CentroCusto>
{
    public void Configure(EntityTypeBuilder<CentroCusto> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Nome).IsRequired().HasMaxLength(100);

        builder.HasOne(c => c.Clinica).WithMany(cl => cl.CentrosCusto)
            .HasForeignKey(c => c.ClinicaId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class LogAuditoriaConfiguration : IEntityTypeConfiguration<LogAuditoria>
{
    public void Configure(EntityTypeBuilder<LogAuditoria> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Acao).IsRequired().HasMaxLength(50);
        builder.Property(a => a.Entidade).IsRequired().HasMaxLength(100);
        builder.Property(a => a.EntidadeId).IsRequired().HasMaxLength(50);
        builder.Property(a => a.EnderecoIp).HasMaxLength(50);

        builder.HasOne(a => a.Usuario).WithMany(u => u.LogsAuditoria)
            .HasForeignKey(a => a.UsuarioId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => new { a.ClinicaId, a.CriadoEm });
        builder.HasIndex(a => a.Entidade);
    }
}
