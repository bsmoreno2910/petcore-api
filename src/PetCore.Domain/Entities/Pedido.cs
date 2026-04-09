using PetCore.Domain.Enums;

namespace PetCore.Domain.Entities;

public class Pedido
{
    public Guid Id { get; set; }
    public Guid ClinicaId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public TipoPedido Tipo { get; set; }
    public StatusPedido Status { get; set; } = StatusPedido.Rascunho;
    public string? Periodo { get; set; }
    public string? Observacoes { get; set; }
    public string? Justificativa { get; set; }

    public Guid CriadoPorId { get; set; }
    public Guid? AprovadoPorId { get; set; }
    public DateTime? DataAprovacao { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }

    public Clinica Clinica { get; set; } = null!;
    public Usuario CriadoPor { get; set; } = null!;
    public Usuario? AprovadoPor { get; set; }
    public ICollection<ItemPedido> Itens { get; set; } = [];
    public ICollection<Movimentacao> Movimentacoes { get; set; } = [];
}
