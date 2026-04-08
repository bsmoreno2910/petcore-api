using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;

namespace PetCore.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Clinic> Clinics => Set<Clinic>();
    public DbSet<ClinicUser> ClinicUsers => Set<ClinicUser>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Species> Species => Set<Species>();
    public DbSet<Breed> Breeds => Set<Breed>();
    public DbSet<Tutor> Tutors => Set<Tutor>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<Hospitalization> Hospitalizations => Set<Hospitalization>();
    public DbSet<HospitalizationEvolution> HospitalizationEvolutions => Set<HospitalizationEvolution>();
    public DbSet<ExamType> ExamTypes => Set<ExamType>();
    public DbSet<ExamRequest> ExamRequests => Set<ExamRequest>();
    public DbSet<ExamResult> ExamResults => Set<ExamResult>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<ProductUnit> ProductUnits => Set<ProductUnit>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Movement> Movements => Set<Movement>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<FinancialCategory> FinancialCategories => Set<FinancialCategory>();
    public DbSet<FinancialTransaction> FinancialTransactions => Set<FinancialTransaction>();
    public DbSet<TransactionInstallment> TransactionInstallments => Set<TransactionInstallment>();
    public DbSet<CostCenter> CostCenters => Set<CostCenter>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
