using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class ServicoInternacao
{
    private readonly AppDbContext _db;

    public ServicoInternacao(AppDbContext db) { _db = db; }

    private IQueryable<Internacao> QueryBase(Guid clinicaId) =>
        _db.Internacoes.Include(h => h.Paciente).ThenInclude(p => p.Tutor)
            .Include(h => h.Paciente).ThenInclude(p => p.Especie)
            .Include(h => h.Veterinario).Include(h => h.Evolucoes)
            .Where(h => h.ClinicaId == clinicaId);

    public async Task<(List<Internacao> Itens, int Total)> ListarPaginadoAsync(
        Guid clinicaId, int pagina, int tamanhoPagina, StatusInternacao? status, Guid? pacienteId)
    {
        var query = QueryBase(clinicaId);
        if (status.HasValue) query = query.Where(h => h.Status == status.Value);
        if (pacienteId.HasValue) query = query.Where(h => h.PacienteId == pacienteId.Value);
        var total = await query.CountAsync();
        var itens = await query.OrderByDescending(h => h.DataInternacao)
            .Skip((pagina - 1) * tamanhoPagina).Take(tamanhoPagina).ToListAsync();
        return (itens, total);
    }

    public async Task<Internacao?> ObterPorIdAsync(Guid id, Guid clinicaId) =>
        await QueryBase(clinicaId).Include(h => h.Evolucoes).ThenInclude(e => e.Veterinario)
            .FirstOrDefaultAsync(h => h.Id == id);

    public async Task<Internacao> CriarAsync(Guid clinicaId, Guid veterinarioId, Guid pacienteId,
        string? motivo, string? baia, string? dieta, string? obs, DateTime? dataInternacao)
    {
        var internacao = new Internacao
        {
            Id = Guid.NewGuid(), ClinicaId = clinicaId, PacienteId = pacienteId,
            VeterinarioId = veterinarioId, Status = StatusInternacao.Ativo,
            Motivo = motivo, Baia = baia, Dieta = dieta, Observacoes = obs,
            DataInternacao = dataInternacao ?? DateTime.UtcNow,
            CriadoEm = DateTime.UtcNow, AtualizadoEm = DateTime.UtcNow
        };
        _db.Internacoes.Add(internacao);
        await _db.SaveChangesAsync();
        return (await ObterPorIdAsync(internacao.Id, clinicaId))!;
    }

    public async Task<Internacao?> DarAltaAsync(Guid id, Guid clinicaId, string? observacoesAlta)
    {
        var h = await _db.Internacoes.FirstOrDefaultAsync(x => x.Id == id && x.ClinicaId == clinicaId);
        if (h == null) return null;
        if (h.Status != StatusInternacao.Ativo) throw new InvalidOperationException("Apenas internações ativas podem receber alta.");
        h.Status = StatusInternacao.Alta;
        h.DataAlta = DateTime.UtcNow;
        h.ObservacoesAlta = observacoesAlta;
        h.AtualizadoEm = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return await ObterPorIdAsync(id, clinicaId);
    }

    public async Task<Evolucao> AdicionarEvolucaoAsync(Guid internacaoId, Guid clinicaId, Guid veterinarioId, Evolucao evolucao)
    {
        var h = await _db.Internacoes.FirstOrDefaultAsync(x => x.Id == internacaoId && x.ClinicaId == clinicaId)
            ?? throw new InvalidOperationException("Internação não encontrada.");
        if (h.Status != StatusInternacao.Ativo) throw new InvalidOperationException("Apenas internações ativas podem receber evolução.");

        evolucao.Id = Guid.NewGuid();
        evolucao.InternacaoId = internacaoId;
        evolucao.VeterinarioId = veterinarioId;
        evolucao.CriadoEm = DateTime.UtcNow;
        _db.Evolucoes.Add(evolucao);
        await _db.SaveChangesAsync();
        return await _db.Evolucoes.Include(e => e.Veterinario).FirstAsync(e => e.Id == evolucao.Id);
    }

    public async Task<List<Evolucao>> ListarEvolucoesAsync(Guid internacaoId, Guid clinicaId)
    {
        var h = await _db.Internacoes.FirstOrDefaultAsync(x => x.Id == internacaoId && x.ClinicaId == clinicaId);
        if (h == null) return [];
        return await _db.Evolucoes.Include(e => e.Veterinario)
            .Where(e => e.InternacaoId == internacaoId).OrderByDescending(e => e.CriadoEm).ToListAsync();
    }
}
