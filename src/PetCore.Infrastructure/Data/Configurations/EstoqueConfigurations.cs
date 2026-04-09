using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCore.Domain.Entities;

namespace PetCore.Infrastructure.Data.Configurations;

public class CategoriaProdutoConfiguration : IEntityTypeConfiguration<CategoriaProduto>
{
    public void Configure(EntityTypeBuilder<CategoriaProduto> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Nome).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Cor).HasMaxLength(20);
    }
}

public class UnidadeProdutoConfiguration : IEntityTypeConfiguration<UnidadeProduto>
{
    public void Configure(EntityTypeBuilder<UnidadeProduto> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Sigla).IsRequired().HasMaxLength(10);
        builder.Property(u => u.Nome).IsRequired().HasMaxLength(50);
    }
}

public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Nome).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Apresentacao).HasMaxLength(200);
        builder.Property(p => p.PrecoCusto).HasPrecision(10, 2);
        builder.Property(p => p.PrecoVenda).HasPrecision(10, 2);
        builder.Property(p => p.Localizacao).HasMaxLength(100);
        builder.Property(p => p.CodigoBarras).HasMaxLength(50);
        builder.Property(p => p.Lote).HasMaxLength(50);

        builder.HasOne(p => p.Clinica).WithMany(c => c.Produtos)
            .HasForeignKey(p => p.ClinicaId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.Categoria).WithMany(c => c.Produtos)
            .HasForeignKey(p => p.CategoriaId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.Unidade).WithMany(u => u.Produtos)
            .HasForeignKey(p => p.UnidadeId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => new { p.ClinicaId, p.CodigoBarras }).IsUnique().HasFilter("\"codigoBarras\" IS NOT NULL");
        builder.HasIndex(p => new { p.ClinicaId, p.Nome });
    }
}

public class MovimentacaoConfiguration : IEntityTypeConfiguration<Movimentacao>
{
    public void Configure(EntityTypeBuilder<Movimentacao> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Tipo).HasConversion<string>().HasMaxLength(20);
        builder.Property(m => m.Motivo).HasMaxLength(300);

        builder.HasOne(m => m.Produto).WithMany(p => p.Movimentacoes)
            .HasForeignKey(m => m.ProdutoId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(m => m.CriadoPor).WithMany(u => u.MovimentacoesCriadas)
            .HasForeignKey(m => m.CriadoPorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(m => m.AprovadoPor).WithMany(u => u.MovimentacoesAprovadas)
            .HasForeignKey(m => m.AprovadoPorId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(m => m.Pedido).WithMany(o => o.Movimentacoes)
            .HasForeignKey(m => m.PedidoId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Codigo).IsRequired().HasMaxLength(30);
        builder.Property(o => o.Tipo).HasConversion<string>().HasMaxLength(20);
        builder.Property(o => o.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(o => o.Periodo).HasMaxLength(50);

        builder.HasOne(o => o.Clinica).WithMany()
            .HasForeignKey(o => o.ClinicaId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(o => o.CriadoPor).WithMany(u => u.PedidosCriados)
            .HasForeignKey(o => o.CriadoPorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(o => o.AprovadoPor).WithMany(u => u.PedidosAprovados)
            .HasForeignKey(o => o.AprovadoPorId).OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(o => o.Codigo).IsUnique();
    }
}

public class ItemPedidoConfiguration : IEntityTypeConfiguration<ItemPedido>
{
    public void Configure(EntityTypeBuilder<ItemPedido> builder)
    {
        builder.HasKey(i => i.Id);

        builder.HasOne(i => i.Pedido).WithMany(o => o.Itens)
            .HasForeignKey(i => i.PedidoId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(i => i.Produto).WithMany(p => p.ItensPedido)
            .HasForeignKey(i => i.ProdutoId).OnDelete(DeleteBehavior.Restrict);
    }
}
