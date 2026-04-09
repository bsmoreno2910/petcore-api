namespace PetCore.Domain.Entities;

public class Evolucao
{
    public Guid Id { get; set; }
    public Guid InternacaoId { get; set; }
    public Guid VeterinarioId { get; set; }

    public decimal? Peso { get; set; }
    public decimal? Temperatura { get; set; }
    public int? FrequenciaCardiaca { get; set; }
    public int? FrequenciaRespiratoria { get; set; }
    public string? Descricao { get; set; }
    public string? Medicamentos { get; set; }
    public string? Alimentacao { get; set; }
    public string? Observacoes { get; set; }

    public DateTime CriadoEm { get; set; }

    public Internacao Internacao { get; set; } = null!;
    public Usuario Veterinario { get; set; } = null!;
}
