using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class ServicoPaciente
{
    private readonly AppDbContext _db;

    public ServicoPaciente(AppDbContext db) { _db = db; }

    public async Task<(List<Paciente> Itens, int Total)> ListarPaginadoAsync(
        Guid clinicaId, int pagina, int tamanhoPagina, string? busca, Guid? especieId, Guid? tutorId)
    {
        var query = _db.Pacientes.Include(p => p.Tutor).Include(p => p.Especie).Include(p => p.Raca)
            .Where(p => p.ClinicaId == clinicaId);

        if (!string.IsNullOrWhiteSpace(busca))
            query = query.Where(p => p.Nome.Contains(busca) || p.Tutor.Nome.Contains(busca));
        if (especieId.HasValue) query = query.Where(p => p.EspecieId == especieId.Value);
        if (tutorId.HasValue) query = query.Where(p => p.TutorId == tutorId.Value);

        var total = await query.CountAsync();
        var itens = await query.OrderBy(p => p.Nome)
            .Skip((pagina - 1) * tamanhoPagina).Take(tamanhoPagina).ToListAsync();
        return (itens, total);
    }

    public async Task<Paciente?> ObterPorIdAsync(Guid id, Guid clinicaId) =>
        await _db.Pacientes.Include(p => p.Tutor).Include(p => p.Especie).Include(p => p.Raca)
            .FirstOrDefaultAsync(p => p.Id == id && p.ClinicaId == clinicaId);

    public async Task<Paciente> CriarAsync(Paciente paciente, Guid clinicaId)
    {
        paciente.Id = Guid.NewGuid();
        paciente.ClinicaId = clinicaId;
        paciente.CriadoEm = DateTime.UtcNow;
        paciente.AtualizadoEm = DateTime.UtcNow;
        _db.Pacientes.Add(paciente);
        await _db.SaveChangesAsync();
        return (await ObterPorIdAsync(paciente.Id, clinicaId))!;
    }

    public async Task<Paciente?> AtualizarAsync(Guid id, Guid clinicaId, Action<Paciente> acao)
    {
        var paciente = await _db.Pacientes.Include(p => p.Tutor).Include(p => p.Especie).Include(p => p.Raca)
            .FirstOrDefaultAsync(p => p.Id == id && p.ClinicaId == clinicaId);
        if (paciente == null) return null;
        acao(paciente);
        paciente.AtualizadoEm = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return paciente;
    }

    public async Task<List<Prontuario>> ListarProntuariosAsync(Guid pacienteId, Guid clinicaId) =>
        await _db.Prontuarios.Include(m => m.Veterinario).Include(m => m.Prescricoes)
            .Where(m => m.PacienteId == pacienteId && m.ClinicaId == clinicaId)
            .OrderByDescending(m => m.CriadoEm).ToListAsync();

    public async Task<List<SolicitacaoExame>> ListarExamesAsync(Guid pacienteId, Guid clinicaId) =>
        await _db.SolicitacoesExame.Include(e => e.TipoExame).Include(e => e.SolicitadoPor).Include(e => e.Resultado)
            .Where(e => e.PacienteId == pacienteId && e.ClinicaId == clinicaId)
            .OrderByDescending(e => e.CriadoEm).ToListAsync();

    public async Task<List<Internacao>> ListarInternacoesAsync(Guid pacienteId, Guid clinicaId) =>
        await _db.Internacoes.Include(h => h.Veterinario)
            .Where(h => h.PacienteId == pacienteId && h.ClinicaId == clinicaId)
            .OrderByDescending(h => h.CriadoEm).ToListAsync();
}
