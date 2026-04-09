namespace PetCore.Domain.Entities;

public class ItemPedido
{
    public Guid Id { get; set; }
    public Guid PedidoId { get; set; }
    public Guid ProdutoId { get; set; }
    public int QuantidadeSolicitada { get; set; }
    public int? QuantidadeAprovada { get; set; }
    public int QuantidadeRecebida { get; set; }
    public string? Observacoes { get; set; }

    public Pedido Pedido { get; set; } = null!;
    public Produto Produto { get; set; } = null!;
}
