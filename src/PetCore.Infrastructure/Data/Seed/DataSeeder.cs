using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Domain.Enums;

namespace PetCore.Infrastructure.Data.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Users.AnyAsync())
            return;

        // SuperAdmin user
        var superAdmin = new User
        {
            Id = Guid.NewGuid(),
            Name = "Administrador PetCore",
            Email = "admin@petcore.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Active = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Vet user
        var vet = new User
        {
            Id = Guid.NewGuid(),
            Name = "Dr. João Silva",
            Email = "joao@petcore.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Vet@123"),
            Crmv = "CRMV-SP 12345",
            Active = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Receptionist
        var receptionist = new User
        {
            Id = Guid.NewGuid(),
            Name = "Maria Santos",
            Email = "maria@petcore.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Rec@123"),
            Active = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        db.Users.AddRange(superAdmin, vet, receptionist);

        // Clinic
        var clinic = new Clinic
        {
            Id = Guid.NewGuid(),
            Name = "PetCore Clínica Central",
            TradeName = "PetCore Central",
            Phone = "(11) 99999-0000",
            Email = "contato@petcore.com",
            City = "São Paulo",
            State = "SP",
            Active = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        db.Clinics.Add(clinic);

        // ClinicUser bindings
        db.ClinicUsers.AddRange(
            new ClinicUser { Id = Guid.NewGuid(), ClinicId = clinic.Id, UserId = superAdmin.Id, Role = UserRole.SuperAdmin, CreatedAt = DateTime.UtcNow },
            new ClinicUser { Id = Guid.NewGuid(), ClinicId = clinic.Id, UserId = vet.Id, Role = UserRole.Veterinarian, CreatedAt = DateTime.UtcNow },
            new ClinicUser { Id = Guid.NewGuid(), ClinicId = clinic.Id, UserId = receptionist.Id, Role = UserRole.Receptionist, CreatedAt = DateTime.UtcNow }
        );

        // Species & Breeds (globais)
        var canino = new Species { Id = Guid.NewGuid(), Name = "Canino" };
        var felino = new Species { Id = Guid.NewGuid(), Name = "Felino" };
        var ave = new Species { Id = Guid.NewGuid(), Name = "Ave" };
        var reptil = new Species { Id = Guid.NewGuid(), Name = "Réptil" };
        var roedor = new Species { Id = Guid.NewGuid(), Name = "Roedor" };

        db.Species.AddRange(canino, felino, ave, reptil, roedor);

        db.Breeds.AddRange(
            new Breed { Id = Guid.NewGuid(), SpeciesId = canino.Id, Name = "Labrador Retriever" },
            new Breed { Id = Guid.NewGuid(), SpeciesId = canino.Id, Name = "Golden Retriever" },
            new Breed { Id = Guid.NewGuid(), SpeciesId = canino.Id, Name = "Bulldog" },
            new Breed { Id = Guid.NewGuid(), SpeciesId = canino.Id, Name = "Poodle" },
            new Breed { Id = Guid.NewGuid(), SpeciesId = canino.Id, Name = "Pastor Alemão" },
            new Breed { Id = Guid.NewGuid(), SpeciesId = canino.Id, Name = "SRD (Sem Raça Definida)" },
            new Breed { Id = Guid.NewGuid(), SpeciesId = felino.Id, Name = "Siamês" },
            new Breed { Id = Guid.NewGuid(), SpeciesId = felino.Id, Name = "Persa" },
            new Breed { Id = Guid.NewGuid(), SpeciesId = felino.Id, Name = "Maine Coon" },
            new Breed { Id = Guid.NewGuid(), SpeciesId = felino.Id, Name = "SRD (Sem Raça Definida)" },
            new Breed { Id = Guid.NewGuid(), SpeciesId = ave.Id, Name = "Calopsita" },
            new Breed { Id = Guid.NewGuid(), SpeciesId = ave.Id, Name = "Periquito" },
            new Breed { Id = Guid.NewGuid(), SpeciesId = roedor.Id, Name = "Hamster" },
            new Breed { Id = Guid.NewGuid(), SpeciesId = roedor.Id, Name = "Porquinho-da-índia" }
        );

        // Product Categories (globais)
        db.ProductCategories.AddRange(
            new ProductCategory { Id = Guid.NewGuid(), Name = "Medicamentos", Color = "#3b82f6", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new ProductCategory { Id = Guid.NewGuid(), Name = "Materiais Cirúrgicos", Color = "#ef4444", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new ProductCategory { Id = Guid.NewGuid(), Name = "Materiais de Limpeza", Color = "#22c55e", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new ProductCategory { Id = Guid.NewGuid(), Name = "Vacinas", Color = "#a855f7", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new ProductCategory { Id = Guid.NewGuid(), Name = "Insumos Laboratoriais", Color = "#f97316", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        );

        // Product Units (globais)
        db.ProductUnits.AddRange(
            new ProductUnit { Id = Guid.NewGuid(), Abbreviation = "un", Name = "Unidade", CreatedAt = DateTime.UtcNow },
            new ProductUnit { Id = Guid.NewGuid(), Abbreviation = "cx", Name = "Caixa", CreatedAt = DateTime.UtcNow },
            new ProductUnit { Id = Guid.NewGuid(), Abbreviation = "fr", Name = "Frasco", CreatedAt = DateTime.UtcNow },
            new ProductUnit { Id = Guid.NewGuid(), Abbreviation = "amp", Name = "Ampola", CreatedAt = DateTime.UtcNow },
            new ProductUnit { Id = Guid.NewGuid(), Abbreviation = "ml", Name = "Mililitro", CreatedAt = DateTime.UtcNow },
            new ProductUnit { Id = Guid.NewGuid(), Abbreviation = "kg", Name = "Quilograma", CreatedAt = DateTime.UtcNow }
        );

        // Exam Types (globais)
        db.ExamTypes.AddRange(
            new ExamType { Id = Guid.NewGuid(), Name = "Hemograma Completo", Category = "Laboratorial", DefaultPrice = 80m, CreatedAt = DateTime.UtcNow },
            new ExamType { Id = Guid.NewGuid(), Name = "Bioquímico", Category = "Laboratorial", DefaultPrice = 120m, CreatedAt = DateTime.UtcNow },
            new ExamType { Id = Guid.NewGuid(), Name = "Raio-X", Category = "Imagem", DefaultPrice = 150m, CreatedAt = DateTime.UtcNow },
            new ExamType { Id = Guid.NewGuid(), Name = "Ultrassonografia", Category = "Imagem", DefaultPrice = 200m, CreatedAt = DateTime.UtcNow },
            new ExamType { Id = Guid.NewGuid(), Name = "Urinálise", Category = "Laboratorial", DefaultPrice = 60m, CreatedAt = DateTime.UtcNow }
        );

        await db.SaveChangesAsync();
    }
}
