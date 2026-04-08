namespace PetCore.Domain.Entities;

public class Clinic
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? TradeName { get; set; }
    public string? LegalName { get; set; }
    public string? Cnpj { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? LogoUrl { get; set; }

    public string? Street { get; set; }
    public string? Number { get; set; }
    public string? Complement { get; set; }
    public string? Neighborhood { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }

    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<ClinicUser> ClinicUsers { get; set; } = [];
    public ICollection<Patient> Patients { get; set; } = [];
    public ICollection<Product> Products { get; set; } = [];
    public ICollection<Appointment> Appointments { get; set; } = [];
    public ICollection<FinancialTransaction> Transactions { get; set; } = [];
    public ICollection<CostCenter> CostCenters { get; set; } = [];
}
