using PetCore.Domain.Enums;

namespace PetCore.Domain.Entities;

public class ParcelaTransacao
{
    public Guid Id { get; set; }
    public Guid TransacaoId { get; set; }
    public int NumeroParcela { get; set; }
    public decimal Valor { get; set; }
    public DateTime DataVencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public StatusTransacao Status { get; set; } = StatusTransacao.Pendente;

    public TransacaoFinanceira Transacao { get; set; } = null!;
}
