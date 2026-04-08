using PetCore.Domain.Enums;

namespace PetCore.Domain.Entities;

public class Patient
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public Guid TutorId { get; set; }
    public Guid SpeciesId { get; set; }
    public Guid? BreedId { get; set; }

    public string Name { get; set; } = string.Empty;
    public PatientSex Sex { get; set; } = PatientSex.Unknown;
    public DateTime? BirthDate { get; set; }
    public decimal? Weight { get; set; }
    public string? Color { get; set; }
    public string? Microchip { get; set; }
    public bool Neutered { get; set; }
    public string? Allergies { get; set; }
    public string? Notes { get; set; }
    public string? PhotoUrl { get; set; }
    public bool Active { get; set; } = true;
    public bool Deceased { get; set; }
    public DateTime? DeceasedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Clinic Clinic { get; set; } = null!;
    public Tutor Tutor { get; set; } = null!;
    public Species Species { get; set; } = null!;
    public Breed? Breed { get; set; }
    public ICollection<MedicalRecord> MedicalRecords { get; set; } = [];
    public ICollection<Appointment> Appointments { get; set; } = [];
    public ICollection<ExamRequest> ExamRequests { get; set; } = [];
    public ICollection<Hospitalization> Hospitalizations { get; set; } = [];
}
