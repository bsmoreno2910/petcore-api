using PetCore.Domain.Enums;

namespace PetCore.Domain.Entities;

public class SolicitacaoExame
{
    public Guid Id { get; set; }
    public Guid ClinicaId { get; set; }
    public Guid PacienteId { get; set; }
    public Guid SolicitadoPorId { get; set; }
    public Guid TipoExameId { get; set; }
    public Guid? ProntuarioId { get; set; }

    public StatusExame Status { get; set; } = StatusExame.Solicitado;
    public string? IndicacaoClinica { get; set; }
    public string? Observacoes { get; set; }

    public DateTime DataSolicitacao { get; set; }
    public DateTime? DataColeta { get; set; }
    public DateTime? DataConclusao { get; set; }

    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }

    public Paciente Paciente { get; set; } = null!;
    public Usuario SolicitadoPor { get; set; } = null!;
    public TipoExame TipoExame { get; set; } = null!;
    public Prontuario? Prontuario { get; set; }
    public ResultadoExame? Resultado { get; set; }
    public ICollection<TransacaoFinanceira> Transacoes { get; set; } = [];
}
