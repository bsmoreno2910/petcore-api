namespace PetCore.API.DTOs.Patients;

public class PatientDto
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public Guid TutorId { get; set; }
    public string TutorName { get; set; } = string.Empty;
    public Guid SpeciesId { get; set; }
    public string SpeciesName { get; set; } = string.Empty;
    public Guid? BreedId { get; set; }
    public string? BreedName { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sex { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public decimal? Weight { get; set; }
    public string? Color { get; set; }
    public string? Microchip { get; set; }
    public bool Neutered { get; set; }
    public string? Allergies { get; set; }
    public string? Notes { get; set; }
    public string? PhotoUrl { get; set; }
    public bool Active { get; set; }
    public bool Deceased { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PatientDetailDto : PatientDto
{
    public string? TutorPhone { get; set; }
    public string? TutorEmail { get; set; }
    public DateTime? DeceasedAt { get; set; }
}

public class CreatePatientRequest
{
    public Guid TutorId { get; set; }
    public Guid SpeciesId { get; set; }
    public Guid? BreedId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Sex { get; set; }
    public DateTime? BirthDate { get; set; }
    public decimal? Weight { get; set; }
    public string? Color { get; set; }
    public string? Microchip { get; set; }
    public bool Neutered { get; set; }
    public string? Allergies { get; set; }
    public string? Notes { get; set; }
    public string? PhotoUrl { get; set; }
}

public class UpdatePatientRequest : CreatePatientRequest { }
