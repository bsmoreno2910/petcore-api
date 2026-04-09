using PetCore.Domain.Enums;

namespace PetCore.Domain.Entities;

public class Movimentacao
{
    public Guid Id { get; set; }
    public Guid ClinicaId { get; set; }
    public Guid ProdutoId { get; set; }
    public TipoMovimentacao Tipo { get; set; }
    public int Quantidade { get; set; }
    public int EstoqueAnterior { get; set; }
    public int NovoEstoque { get; set; }
    public string? Motivo { get; set; }
    public string? Observacoes { get; set; }

    public Guid CriadoPorId { get; set; }
    public Guid? AprovadoPorId { get; set; }
    public DateTime? DataAprovacao { get; set; }
    public Guid? PedidoId { get; set; }

    public DateTime CriadoEm { get; set; }

    public Produto Produto { get; set; } = null!;
    public Usuario CriadoPor { get; set; } = null!;
    public Usuario? AprovadoPor { get; set; }
    public Pedido? Pedido { get; set; }
}
