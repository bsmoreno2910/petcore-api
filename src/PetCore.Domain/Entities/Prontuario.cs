namespace PetCore.Domain.Entities;

public class Prontuario
{
    public Guid Id { get; set; }
    public Guid ClinicaId { get; set; }
    public Guid PacienteId { get; set; }
    public Guid VeterinarioId { get; set; }
    public Guid? AgendamentoId { get; set; }

    public string? QueixaPrincipal { get; set; }
    public string? Historico { get; set; }
    public string? Anamnese { get; set; }

    public decimal? Peso { get; set; }
    public decimal? Temperatura { get; set; }
    public int? FrequenciaCardiaca { get; set; }
    public int? FrequenciaRespiratoria { get; set; }
    public string? ExameFisico { get; set; }
    public string? Mucosas { get; set; }
    public string? Hidratacao { get; set; }
    public string? Linfonodos { get; set; }

    public string? Diagnostico { get; set; }
    public string? DiagnosticoDiferencial { get; set; }
    public string? Tratamento { get; set; }
    public string? Observacoes { get; set; }
    public string? NotasInternas { get; set; }

    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }

    public Paciente Paciente { get; set; } = null!;
    public Usuario Veterinario { get; set; } = null!;
    public Agendamento? Agendamento { get; set; }
    public ICollection<Prescricao> Prescricoes { get; set; } = [];
    public ICollection<SolicitacaoExame> SolicitacoesExame { get; set; } = [];
}
