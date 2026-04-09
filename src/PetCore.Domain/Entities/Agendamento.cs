using PetCore.Domain.Enums;

namespace PetCore.Domain.Entities;

public class Agendamento
{
    public Guid Id { get; set; }
    public Guid ClinicaId { get; set; }
    public Guid PacienteId { get; set; }
    public Guid? VeterinarioId { get; set; }
    public TipoAgendamento Tipo { get; set; }
    public StatusAgendamento Status { get; set; } = StatusAgendamento.Agendado;

    public DateTime DataHoraAgendada { get; set; }
    public int DuracaoMinutos { get; set; } = 30;
    public DateTime? IniciadoEm { get; set; }
    public DateTime? FinalizadoEm { get; set; }

    public string? Motivo { get; set; }
    public string? Observacoes { get; set; }
    public string? MotivoCancelamento { get; set; }

    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }

    public Clinica Clinica { get; set; } = null!;
    public Paciente Paciente { get; set; } = null!;
    public Usuario? Veterinario { get; set; }
    public Prontuario? Prontuario { get; set; }
    public ICollection<TransacaoFinanceira> Transacoes { get; set; } = [];
}
