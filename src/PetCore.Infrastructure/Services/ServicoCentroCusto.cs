using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class ServicoCentroCusto
{
    private readonly AppDbContext _db;

    public ServicoCentroCusto(AppDbContext db) { _db = db; }

    public async Task<List<CentroCusto>> ListarTodosAsync(Guid clinicaId) =>
        await _db.CentrosCusto.Where(c => c.ClinicaId == clinicaId).OrderBy(c => c.Nome).ToListAsync();

    public async Task<CentroCusto> CriarAsync(Guid clinicaId, string nome, string? descricao)
    {
        var cc = new CentroCusto { Id = Guid.NewGuid(), ClinicaId = clinicaId, Nome = nome, Descricao = descricao, CriadoEm = DateTime.UtcNow };
        _db.CentrosCusto.Add(cc); await _db.SaveChangesAsync(); return cc;
    }

    public async Task<CentroCusto?> AtualizarAsync(Guid id, Guid clinicaId, string nome, string? descricao)
    {
        var cc = await _db.CentrosCusto.FirstOrDefaultAsync(c => c.Id == id && c.ClinicaId == clinicaId);
        if (cc == null) return null;
        cc.Nome = nome; cc.Descricao = descricao; await _db.SaveChangesAsync(); return cc;
    }

    public async Task<(decimal TotalReceita, decimal TotalDespesa, int Total)> ObterResumoAsync(Guid id, Guid clinicaId)
    {
        var transacoes = await _db.TransacoesFinanceiras
            .Where(t => t.CentroCustoId == id && t.ClinicaId == clinicaId && t.Status != StatusTransacao.Cancelado).ToListAsync();
        return (
            transacoes.Where(t => t.Tipo == TipoTransacao.Receita).Sum(t => t.Valor),
            transacoes.Where(t => t.Tipo == TipoTransacao.Despesa).Sum(t => t.Valor),
            transacoes.Count);
    }

    public async Task<List<(Guid Id, string Nome, decimal Receita, decimal Despesa, int Total)>> ObterRelatorioAsync(Guid clinicaId)
    {
        var centros = await _db.CentrosCusto.Include(c => c.Transacoes).Where(c => c.ClinicaId == clinicaId).ToListAsync();
        return centros.Select(c =>
        {
            var ativas = c.Transacoes.Where(t => t.Status != StatusTransacao.Cancelado).ToList();
            return (c.Id, c.Nome,
                ativas.Where(t => t.Tipo == TipoTransacao.Receita).Sum(t => t.Valor),
                ativas.Where(t => t.Tipo == TipoTransacao.Despesa).Sum(t => t.Valor),
                ativas.Count);
        }).ToList();
    }
}
