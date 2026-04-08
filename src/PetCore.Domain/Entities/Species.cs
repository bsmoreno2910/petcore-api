namespace PetCore.Domain.Entities;

public class Species
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Active { get; set; } = true;
    public ICollection<Breed> Breeds { get; set; } = [];
}
