using PetCore.Domain.Enums;

namespace PetCore.Domain.Entities;

public class Hospitalization
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public Guid PatientId { get; set; }
    public Guid VeterinarianId { get; set; }

    public HospitalizationStatus Status { get; set; } = HospitalizationStatus.Active;
    public string? Reason { get; set; }
    public string? Cage { get; set; }
    public string? Diet { get; set; }
    public string? Notes { get; set; }

    public DateTime AdmittedAt { get; set; }
    public DateTime? DischargedAt { get; set; }
    public string? DischargeNotes { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Patient Patient { get; set; } = null!;
    public User Veterinarian { get; set; } = null!;
    public ICollection<HospitalizationEvolution> Evolutions { get; set; } = [];
    public ICollection<FinancialTransaction> Transactions { get; set; } = [];
}
