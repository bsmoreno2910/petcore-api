namespace PetCore.Domain.Entities;

public class Tutor
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
    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Clinic Clinic { get; set; } = null!;
    public ICollection<Patient> Patients { get; set; } = [];
    public ICollection<FinancialTransaction> Transactions { get; set; } = [];
}
