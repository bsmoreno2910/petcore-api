using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class ServicoTutor
{
    private readonly AppDbContext _db;

    public ServicoTutor(AppDbContext db) { _db = db; }

    public async Task<(List<Tutor> Itens, int Total)> ListarPaginadoAsync(
        Guid clinicaId, int pagina, int tamanhoPagina, string? busca, string? telefone, string? cpf)
    {
        var query = _db.Tutores.Include(t => t.Pacientes).Where(t => t.ClinicaId == clinicaId);

        if (!string.IsNullOrWhiteSpace(busca))
            query = query.Where(t =>
                t.Nome.Contains(busca) ||
                (t.Email != null && t.Email.Contains(busca)) ||
                (t.Cpf != null && t.Cpf.Contains(busca)) ||
                (t.Rg != null && t.Rg.Contains(busca)) ||
                (t.Telefone != null && t.Telefone.Contains(busca)));

        if (!string.IsNullOrWhiteSpace(telefone))
            query = query.Where(t => t.Telefone != null && t.Telefone.Contains(telefone));
        if (!string.IsNullOrWhiteSpace(cpf))
            query = query.Where(t => t.Cpf != null && t.Cpf.Contains(cpf));

        var total = await query.CountAsync();
        var itens = await query.OrderBy(t => t.Nome)
            .Skip((pagina - 1) * tamanhoPagina).Take(tamanhoPagina).ToListAsync();
        return (itens, total);
    }

    public async Task<Tutor?> ObterPorIdAsync(Guid id, Guid clinicaId) =>
        await _db.Tutores
            .Include(t => t.Pacientes).ThenInclude(p => p.Especie)
            .Include(t => t.Pacientes).ThenInclude(p => p.Raca)
            .FirstOrDefaultAsync(t => t.Id == id && t.ClinicaId == clinicaId);

    public async Task<Tutor> CriarAsync(Tutor tutor, Guid clinicaId)
    {
        tutor.Id = Guid.NewGuid();
        tutor.ClinicaId = clinicaId;
        tutor.CriadoEm = DateTime.UtcNow;
        tutor.AtualizadoEm = DateTime.UtcNow;
        _db.Tutores.Add(tutor);
        await _db.SaveChangesAsync();
        return tutor;
    }

    public async Task<Tutor?> AtualizarAsync(Guid id, Guid clinicaId, Action<Tutor> acao)
    {
        var tutor = await _db.Tutores.FirstOrDefaultAsync(t => t.Id == id && t.ClinicaId == clinicaId);
        if (tutor == null) return null;
        acao(tutor);
        tutor.AtualizadoEm = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return tutor;
    }

    public async Task<List<Paciente>> ListarPacientesAsync(Guid tutorId, Guid clinicaId) =>
        await _db.Pacientes.Include(p => p.Especie).Include(p => p.Raca)
            .Where(p => p.TutorId == tutorId && p.ClinicaId == clinicaId).OrderBy(p => p.Nome).ToListAsync();

    public async Task<(decimal TotalReceita, decimal TotalPago, decimal TotalPendente, decimal TotalAtrasado)>
        ObterResumoFinanceiroAsync(Guid tutorId, Guid clinicaId)
    {
        var transacoes = await _db.TransacoesFinanceiras
            .Where(t => t.TutorId == tutorId && t.ClinicaId == clinicaId && t.Tipo == TipoTransacao.Receita).ToListAsync();
        return (
            transacoes.Sum(t => t.Valor),
            transacoes.Where(t => t.Status == StatusTransacao.Pago).Sum(t => t.ValorPago ?? t.Valor),
            transacoes.Where(t => t.Status == StatusTransacao.Pendente).Sum(t => t.Valor),
            transacoes.Where(t => t.Status == StatusTransacao.Atrasado).Sum(t => t.Valor));
    }
}
