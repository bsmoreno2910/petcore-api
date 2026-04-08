namespace PetCore.API.DTOs.Tutors;

public class TutorDto
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Cpf { get; set; }
    public string? Rg { get; set; }
    public string? Phone { get; set; }
    public string? PhoneSecondary { get; set; }
    public string? Email { get; set; }
    public string? Street { get; set; }
    public string? Number { get; set; }
    public string? Complement { get; set; }
    public string? Neighborhood { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? Notes { get; set; }
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
    public int PatientCount { get; set; }
}

public class TutorDetailDto : TutorDto
{
    public List<TutorPatientDto> Patients { get; set; } = [];
}

public class TutorPatientDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SpeciesName { get; set; } = string.Empty;
    public string? BreedName { get; set; }
    public bool Active { get; set; }
}

public class CreateTutorRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Cpf { get; set; }
    public string? Rg { get; set; }
    public string? Phone { get; set; }
    public string? PhoneSecondary { get; set; }
    public string? Email { get; set; }
    public string? Street { get; set; }
    public string? Number { get; set; }
    public string? Complement { get; set; }
    public string? Neighborhood { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? Notes { get; set; }
}

public class UpdateTutorRequest : CreateTutorRequest { }

public class TutorFinancialSummaryDto
{
    public decimal TotalRevenue { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TotalPending { get; set; }
    public decimal TotalOverdue { get; set; }
}
