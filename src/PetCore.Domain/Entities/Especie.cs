namespace PetCore.Domain.Entities;

public class Especie
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public ICollection<Raca> Racas { get; set; } = [];
}
