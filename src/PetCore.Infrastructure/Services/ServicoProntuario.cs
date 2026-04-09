using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class ServicoProntuario
{
    private readonly AppDbContext _db;

    public ServicoProntuario(AppDbContext db) { _db = db; }

    private IQueryable<Prontuario> QueryBase(Guid clinicaId) =>
        _db.Prontuarios.Include(m => m.Paciente).Include(m => m.Veterinario).Include(m => m.Prescricoes)
            .Where(m => m.ClinicaId == clinicaId);

    public async Task<(List<Prontuario> Itens, int Total)> ListarPaginadoAsync(
        Guid clinicaId, int pagina, int tamanhoPagina,
        Guid? pacienteId, Guid? veterinarioId, DateTime? dataInicio, DateTime? dataFim)
    {
        var query = QueryBase(clinicaId);
        if (pacienteId.HasValue) query = query.Where(m => m.PacienteId == pacienteId.Value);
        if (veterinarioId.HasValue) query = query.Where(m => m.VeterinarioId == veterinarioId.Value);
        if (dataInicio.HasValue) query = query.Where(m => m.CriadoEm >= dataInicio.Value);
        if (dataFim.HasValue) query = query.Where(m => m.CriadoEm <= dataFim.Value);

        var total = await query.CountAsync();
        var itens = await query.OrderByDescending(m => m.CriadoEm)
            .Skip((pagina - 1) * tamanhoPagina).Take(tamanhoPagina).ToListAsync();
        return (itens, total);
    }

    public async Task<Prontuario?> ObterPorIdAsync(Guid id, Guid clinicaId) =>
        await QueryBase(clinicaId).Include(m => m.SolicitacoesExame).ThenInclude(e => e.TipoExame)
            .FirstOrDefaultAsync(m => m.Id == id);

    public async Task<Prontuario> CriarAsync(Prontuario prontuario, Guid clinicaId, Guid veterinarioId)
    {
        prontuario.Id = Guid.NewGuid();
        prontuario.ClinicaId = clinicaId;
        prontuario.VeterinarioId = veterinarioId;
        prontuario.CriadoEm = DateTime.UtcNow;
        prontuario.AtualizadoEm = DateTime.UtcNow;
        _db.Prontuarios.Add(prontuario);
        await _db.SaveChangesAsync();
        return (await ObterPorIdAsync(prontuario.Id, clinicaId))!;
    }

    public async Task<Prontuario?> AtualizarAsync(Guid id, Guid clinicaId, Action<Prontuario> acao)
    {
        var p = await _db.Prontuarios.FirstOrDefaultAsync(m => m.Id == id && m.ClinicaId == clinicaId);
        if (p == null) return null;
        acao(p);
        p.AtualizadoEm = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return await ObterPorIdAsync(id, clinicaId);
    }

    public async Task<Prescricao> AdicionarPrescricaoAsync(Guid prontuarioId, Guid clinicaId, Prescricao prescricao)
    {
        var p = await _db.Prontuarios.FirstOrDefaultAsync(m => m.Id == prontuarioId && m.ClinicaId == clinicaId)
            ?? throw new InvalidOperationException("Prontuário não encontrado.");
        prescricao.Id = Guid.NewGuid();
        prescricao.ProntuarioId = prontuarioId;
        prescricao.CriadoEm = DateTime.UtcNow;
        _db.Prescricoes.Add(prescricao);
        await _db.SaveChangesAsync();
        return prescricao;
    }

    public async Task<bool> RemoverPrescricaoAsync(Guid prontuarioId, Guid prescricaoId, Guid clinicaId)
    {
        var p = await _db.Prontuarios.FirstOrDefaultAsync(m => m.Id == prontuarioId && m.ClinicaId == clinicaId);
        if (p == null) return false;
        var presc = await _db.Prescricoes.FirstOrDefaultAsync(x => x.Id == prescricaoId && x.ProntuarioId == prontuarioId);
        if (presc == null) return false;
        _db.Prescricoes.Remove(presc);
        await _db.SaveChangesAsync();
        return true;
    }
}
