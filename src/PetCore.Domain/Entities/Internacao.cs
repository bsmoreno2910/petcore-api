using PetCore.Domain.Enums;

namespace PetCore.Domain.Entities;

public class Internacao
{
    public Guid Id { get; set; }
    public Guid ClinicaId { get; set; }
    public Guid PacienteId { get; set; }
    public Guid VeterinarioId { get; set; }

    public StatusInternacao Status { get; set; } = StatusInternacao.Ativo;
    public string? Motivo { get; set; }
    public string? Baia { get; set; }
    public string? Dieta { get; set; }
    public string? Observacoes { get; set; }

    public DateTime DataInternacao { get; set; }
    public DateTime? DataAlta { get; set; }
    public string? ObservacoesAlta { get; set; }

    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }

    public Paciente Paciente { get; set; } = null!;
    public Usuario Veterinario { get; set; } = null!;
    public ICollection<Evolucao> Evolucoes { get; set; } = [];
    public ICollection<TransacaoFinanceira> Transacoes { get; set; } = [];
}
