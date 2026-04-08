namespace PetCore.Domain.Entities;

public class ProductUnit
{
    public Guid Id { get; set; }
    public string Abbreviation { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public ICollection<Product> Products { get; set; } = [];
}
