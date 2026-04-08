namespace PetCore.Domain.Entities;

public class Breed
{
    public Guid Id { get; set; }
    public Guid SpeciesId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Active { get; set; } = true;
    public Species Species { get; set; } = null!;
}
