namespace PetCore.Domain.Entities;

public class Produto
{
    public Guid Id { get; set; }
    public Guid ClinicaId { get; set; }
    public Guid CategoriaId { get; set; }
    public Guid UnidadeId { get; set; }

    public string Nome { get; set; } = string.Empty;
    public string? Apresentacao { get; set; }
    public int EstoqueAtual { get; set; }
    public int EstoqueMinimo { get; set; }
    public int? EstoqueMaximo { get; set; }
    public decimal? PrecoCusto { get; set; }
    public decimal? PrecoVenda { get; set; }
    public string? Localizacao { get; set; }
    public string? CodigoBarras { get; set; }
    public string? Lote { get; set; }
    public DateTime? DataValidade { get; set; }
    public bool Ativo { get; set; } = true;
    public string? Observacoes { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }

    public Clinica Clinica { get; set; } = null!;
    public CategoriaProduto Categoria { get; set; } = null!;
    public UnidadeProduto Unidade { get; set; } = null!;
    public ICollection<Movimentacao> Movimentacoes { get; set; } = [];
    public ICollection<ItemPedido> ItensPedido { get; set; } = [];
}
