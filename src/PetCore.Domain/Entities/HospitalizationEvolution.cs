namespace PetCore.Domain.Entities;

public class HospitalizationEvolution
{
    public Guid Id { get; set; }
    public Guid HospitalizationId { get; set; }
    public Guid VeterinarianId { get; set; }

    public decimal? Weight { get; set; }
    public decimal? Temperature { get; set; }
    public int? HeartRate { get; set; }
    public int? RespiratoryRate { get; set; }
    public string? Description { get; set; }
    public string? Medications { get; set; }
    public string? Feeding { get; set; }
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public Hospitalization Hospitalization { get; set; } = null!;
    public User Veterinarian { get; set; } = null!;
}
