namespace PetCore.Domain.Entities;

public class Raca
{
    public Guid Id { get; set; }
    public Guid EspecieId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public Especie Especie { get; set; } = null!;
}
