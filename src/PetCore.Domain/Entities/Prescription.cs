namespace PetCore.Domain.Entities;

public class Prescription
{
    public Guid Id { get; set; }
    public Guid MedicalRecordId { get; set; }
    public string MedicineName { get; set; } = string.Empty;
    public string? Dosage { get; set; }
    public string? Frequency { get; set; }
    public string? Duration { get; set; }
    public string? Route { get; set; }
    public string? Instructions { get; set; }
    public int? Quantity { get; set; }
    public DateTime CreatedAt { get; set; }

    public MedicalRecord MedicalRecord { get; set; } = null!;
}
