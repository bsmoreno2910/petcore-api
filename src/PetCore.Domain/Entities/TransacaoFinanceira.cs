using PetCore.Domain.Enums;

namespace PetCore.Domain.Entities;

public class TransacaoFinanceira
{
    public Guid Id { get; set; }
    public Guid ClinicaId { get; set; }
    public TipoTransacao Tipo { get; set; }
    public StatusTransacao Status { get; set; } = StatusTransacao.Pendente;
    public Guid CategoriaFinanceiraId { get; set; }

    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public decimal? Desconto { get; set; }
    public decimal? ValorPago { get; set; }
    public MetodoPagamento? MetodoPagamento { get; set; }

    public DateTime DataVencimento { get; set; }
    public DateTime? DataPagamento { get; set; }

    public Guid? TutorId { get; set; }
    public Guid? AgendamentoId { get; set; }
    public Guid? InternacaoId { get; set; }
    public Guid? SolicitacaoExameId { get; set; }
    public Guid? CentroCustoId { get; set; }

    public string? Observacoes { get; set; }
    public string? NumeroNota { get; set; }

    public Guid CriadoPorId { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }

    public Clinica Clinica { get; set; } = null!;
    public CategoriaFinanceira CategoriaFinanceira { get; set; } = null!;
    public Tutor? Tutor { get; set; }
    public Agendamento? Agendamento { get; set; }
    public Internacao? Internacao { get; set; }
    public SolicitacaoExame? SolicitacaoExame { get; set; }
    public CentroCusto? CentroCusto { get; set; }
    public Usuario CriadoPor { get; set; } = null!;
    public ICollection<ParcelaTransacao> Parcelas { get; set; } = [];
}
