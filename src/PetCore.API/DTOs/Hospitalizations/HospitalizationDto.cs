namespace PetCore.API.DTOs.Hospitalizations;

public class HospitalizationDto
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string TutorName { get; set; } = string.Empty;
    public string SpeciesName { get; set; } = string.Empty;
    public Guid VeterinarianId { get; set; }
    public string VeterinarianName { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public string? Cage { get; set; }
    public string? Diet { get; set; }
    public string? Notes { get; set; }

    public DateTime AdmittedAt { get; set; }
    public DateTime? DischargedAt { get; set; }
    public string? DischargeNotes { get; set; }

    public DateTime CreatedAt { get; set; }
    public int EvolutionCount { get; set; }
}

public class HospitalizationDetailDto : HospitalizationDto
{
    public List<EvolutionDto> Evolutions { get; set; } = [];
}

public class CreateHospitalizationRequest
{
    public Guid PatientId { get; set; }
    public string? Reason { get; set; }
    public string? Cage { get; set; }
    public string? Diet { get; set; }
    public string? Notes { get; set; }
    public DateTime? AdmittedAt { get; set; }
}

public class UpdateHospitalizationRequest
{
    public string? Cage { get; set; }
    public string? Diet { get; set; }
    public string? Notes { get; set; }
}

public class DischargeRequest
{
    public string? DischargeNotes { get; set; }
}

public class EvolutionDto
{
    public Guid Id { get; set; }
    public Guid HospitalizationId { get; set; }
    public Guid VeterinarianId { get; set; }
    public string VeterinarianName { get; set; } = string.Empty;

    public decimal? Weight { get; set; }
    public decimal? Temperature { get; set; }
    public int? HeartRate { get; set; }
    public int? RespiratoryRate { get; set; }
    public string? Description { get; set; }
    public string? Medications { get; set; }
    public string? Feeding { get; set; }
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
}

public class CreateEvolutionRequest
{
    public decimal? Weight { get; set; }
    public decimal? Temperature { get; set; }
    public int? HeartRate { get; set; }
    public int? RespiratoryRate { get; set; }
    public string? Description { get; set; }
    public string? Medications { get; set; }
    public string? Feeding { get; set; }
    public string? Notes { get; set; }
}
