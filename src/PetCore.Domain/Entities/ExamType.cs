namespace PetCore.Domain.Entities;

public class ExamType
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public decimal? DefaultPrice { get; set; }
    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}
