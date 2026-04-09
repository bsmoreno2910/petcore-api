using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class ServicoEspecie
{
    private readonly AppDbContext _db;

    public ServicoEspecie(AppDbContext db) { _db = db; }

    public async Task<List<Especie>> ListarTodasAsync() =>
        await _db.Especies.Include(s => s.Racas.Where(r => r.Ativo)).OrderBy(s => s.Nome).ToListAsync();

    public async Task<Especie> CriarAsync(string nome)
    {
        if (await _db.Especies.AnyAsync(s => s.Nome == nome))
            throw new InvalidOperationException("Espécie já cadastrada.");
        var especie = new Especie { Id = Guid.NewGuid(), Nome = nome };
        _db.Especies.Add(especie);
        await _db.SaveChangesAsync();
        return especie;
    }

    public async Task<List<Raca>> ListarRacasAsync(Guid especieId) =>
        await _db.Racas.Where(r => r.EspecieId == especieId).OrderBy(r => r.Nome).ToListAsync();

    public async Task<Raca> CriarRacaAsync(Guid especieId, string nome)
    {
        if (!await _db.Especies.AnyAsync(s => s.Id == especieId))
            throw new InvalidOperationException("Espécie não encontrada.");
        var raca = new Raca { Id = Guid.NewGuid(), EspecieId = especieId, Nome = nome };
        _db.Racas.Add(raca);
        await _db.SaveChangesAsync();
        return raca;
    }
}
