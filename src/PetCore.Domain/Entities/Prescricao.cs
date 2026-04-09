namespace PetCore.Domain.Entities;

public class Prescricao
{
    public Guid Id { get; set; }
    public Guid ProntuarioId { get; set; }
    public string NomeMedicamento { get; set; } = string.Empty;
    public string? Dosagem { get; set; }
    public string? Frequencia { get; set; }
    public string? Duracao { get; set; }
    public string? ViaAdministracao { get; set; }
    public string? Instrucoes { get; set; }
    public int? Quantidade { get; set; }
    public DateTime CriadoEm { get; set; }

    public Prontuario Prontuario { get; set; } = null!;
}
