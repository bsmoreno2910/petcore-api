using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class SpeciesService
{
    private readonly AppDbContext _db;

    public SpeciesService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Species>> GetAllAsync()
    {
        return await _db.Species
            .Include(s => s.Breeds.Where(b => b.Active))
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<Species> CreateAsync(string name)
    {
        var exists = await _db.Species.AnyAsync(s => s.Name == name);
        if (exists)
            throw new InvalidOperationException("Espécie já cadastrada.");

        var species = new Species
        {
            Id = Guid.NewGuid(),
            Name = name
        };

        _db.Species.Add(species);
        await _db.SaveChangesAsync();
        return species;
    }

    public async Task<List<Breed>> GetBreedsAsync(Guid speciesId)
    {
        return await _db.Breeds
            .Where(b => b.SpeciesId == speciesId)
            .OrderBy(b => b.Name)
            .ToListAsync();
    }

    public async Task<Breed> CreateBreedAsync(Guid speciesId, string name)
    {
        var speciesExists = await _db.Species.AnyAsync(s => s.Id == speciesId);
        if (!speciesExists)
            throw new InvalidOperationException("Espécie não encontrada.");

        var breed = new Breed
        {
            Id = Guid.NewGuid(),
            SpeciesId = speciesId,
            Name = name
        };

        _db.Breeds.Add(breed);
        await _db.SaveChangesAsync();
        return breed;
    }
}
