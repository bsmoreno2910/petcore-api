using PetCore.Domain.Enums;

namespace PetCore.Domain.Entities;

public class Paciente
{
    public Guid Id { get; set; }
    public Guid ClinicaId { get; set; }
    public Guid TutorId { get; set; }
    public Guid EspecieId { get; set; }
    public Guid? RacaId { get; set; }

    public string Nome { get; set; } = string.Empty;
    public SexoPaciente Sexo { get; set; } = SexoPaciente.Desconhecido;
    public DateTime? DataNascimento { get; set; }
    public decimal? Peso { get; set; }
    public string? Cor { get; set; }
    public string? Microchip { get; set; }
    public bool Castrado { get; set; }
    public string? Alergias { get; set; }
    public string? Observacoes { get; set; }
    public string? FotoUrl { get; set; }
    public bool Ativo { get; set; } = true;
    public bool Obito { get; set; }
    public DateTime? DataObito { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }

    public Clinica Clinica { get; set; } = null!;
    public Tutor Tutor { get; set; } = null!;
    public Especie Especie { get; set; } = null!;
    public Raca? Raca { get; set; }
    public ICollection<Prontuario> Prontuarios { get; set; } = [];
    public ICollection<Agendamento> Agendamentos { get; set; } = [];
    public ICollection<SolicitacaoExame> SolicitacoesExame { get; set; } = [];
    public ICollection<Internacao> Internacoes { get; set; } = [];
}
