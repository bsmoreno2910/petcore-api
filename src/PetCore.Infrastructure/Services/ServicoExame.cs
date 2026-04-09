using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class ServicoExame
{
    private readonly AppDbContext _db;

    public ServicoExame(AppDbContext db) { _db = db; }

    public async Task<List<TipoExame>> ListarTiposAsync() =>
        await _db.TiposExame.OrderBy(e => e.Nome).ToListAsync();

    public async Task<TipoExame> CriarTipoAsync(string nome, string? categoria, decimal? precoDefault)
    {
        var tipo = new TipoExame { Id = Guid.NewGuid(), Nome = nome, Categoria = categoria, PrecoDefault = precoDefault, CriadoEm = DateTime.UtcNow };
        _db.TiposExame.Add(tipo);
        await _db.SaveChangesAsync();
        return tipo;
    }

    public async Task<TipoExame?> AtualizarTipoAsync(Guid id, string nome, string? categoria, decimal? precoDefault)
    {
        var tipo = await _db.TiposExame.FindAsync(id);
        if (tipo == null) return null;
        tipo.Nome = nome; tipo.Categoria = categoria; tipo.PrecoDefault = precoDefault;
        await _db.SaveChangesAsync();
        return tipo;
    }

    private IQueryable<SolicitacaoExame> QueryBase(Guid clinicaId) =>
        _db.SolicitacoesExame.Include(e => e.Paciente).ThenInclude(p => p.Tutor)
            .Include(e => e.SolicitadoPor).Include(e => e.TipoExame)
            .Include(e => e.Resultado).ThenInclude(r => r!.RealizadoPor)
            .Where(e => e.ClinicaId == clinicaId);

    public async Task<(List<SolicitacaoExame> Itens, int Total)> ListarSolicitacoesPaginadoAsync(
        Guid clinicaId, int pagina, int tamanhoPagina,
        StatusExame? status, Guid? pacienteId, Guid? tipoExameId, DateTime? dataInicio, DateTime? dataFim)
    {
        var query = QueryBase(clinicaId);
        if (status.HasValue) query = query.Where(e => e.Status == status.Value);
        if (pacienteId.HasValue) query = query.Where(e => e.PacienteId == pacienteId.Value);
        if (tipoExameId.HasValue) query = query.Where(e => e.TipoExameId == tipoExameId.Value);
        if (dataInicio.HasValue) query = query.Where(e => e.DataSolicitacao >= dataInicio.Value);
        if (dataFim.HasValue) query = query.Where(e => e.DataSolicitacao <= dataFim.Value);

        var total = await query.CountAsync();
        var itens = await query.OrderByDescending(e => e.DataSolicitacao)
            .Skip((pagina - 1) * tamanhoPagina).Take(tamanhoPagina).ToListAsync();
        return (itens, total);
    }

    public async Task<SolicitacaoExame?> ObterSolicitacaoPorIdAsync(Guid id, Guid clinicaId) =>
        await QueryBase(clinicaId).FirstOrDefaultAsync(e => e.Id == id);

    public async Task<SolicitacaoExame> CriarSolicitacaoAsync(Guid clinicaId, Guid solicitadoPorId, Guid pacienteId,
        Guid tipoExameId, Guid? prontuarioId, string? indicacaoClinica, string? obs)
    {
        var sol = new SolicitacaoExame
        {
            Id = Guid.NewGuid(), ClinicaId = clinicaId, PacienteId = pacienteId,
            SolicitadoPorId = solicitadoPorId, TipoExameId = tipoExameId,
            ProntuarioId = prontuarioId, IndicacaoClinica = indicacaoClinica, Observacoes = obs,
            Status = StatusExame.Solicitado, DataSolicitacao = DateTime.UtcNow,
            CriadoEm = DateTime.UtcNow, AtualizadoEm = DateTime.UtcNow
        };
        _db.SolicitacoesExame.Add(sol);
        await _db.SaveChangesAsync();
        return (await ObterSolicitacaoPorIdAsync(sol.Id, clinicaId))!;
    }

    public async Task<SolicitacaoExame?> ColetarAsync(Guid id, Guid clinicaId)
    {
        var sol = await _db.SolicitacoesExame.FirstOrDefaultAsync(e => e.Id == id && e.ClinicaId == clinicaId);
        if (sol == null) return null;
        if (sol.Status != StatusExame.Solicitado) throw new InvalidOperationException("Apenas exames solicitados podem ter coleta registrada.");
        sol.Status = StatusExame.AmostraColetada;
        sol.DataColeta = DateTime.UtcNow;
        sol.AtualizadoEm = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return await ObterSolicitacaoPorIdAsync(id, clinicaId);
    }

    public async Task<SolicitacaoExame?> CancelarAsync(Guid id, Guid clinicaId)
    {
        var sol = await _db.SolicitacoesExame.FirstOrDefaultAsync(e => e.Id == id && e.ClinicaId == clinicaId);
        if (sol == null) return null;
        if (sol.Status == StatusExame.Concluido || sol.Status == StatusExame.Cancelado)
            throw new InvalidOperationException("Exame já finalizado ou cancelado.");
        sol.Status = StatusExame.Cancelado;
        sol.AtualizadoEm = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return await ObterSolicitacaoPorIdAsync(id, clinicaId);
    }

    public async Task<ResultadoExame> AdicionarResultadoAsync(Guid solicitacaoId, Guid clinicaId, Guid realizadoPorId,
        string? texto, string? arquivoUrl, string? valoresRef, string? obs, string? conclusao)
    {
        var sol = await _db.SolicitacoesExame.FirstOrDefaultAsync(e => e.Id == solicitacaoId && e.ClinicaId == clinicaId)
            ?? throw new InvalidOperationException("Solicitação de exame não encontrada.");
        if (sol.Status == StatusExame.Concluido) throw new InvalidOperationException("Exame já possui resultado.");
        if (sol.Status == StatusExame.Cancelado) throw new InvalidOperationException("Exame cancelado não pode receber resultado.");

        var resultado = new ResultadoExame
        {
            Id = Guid.NewGuid(), SolicitacaoExameId = solicitacaoId, RealizadoPorId = realizadoPorId,
            TextoResultado = texto, ArquivoResultadoUrl = arquivoUrl, ValoresReferencia = valoresRef,
            Observacoes = obs, Conclusao = conclusao, CriadoEm = DateTime.UtcNow
        };
        sol.Status = StatusExame.Concluido;
        sol.DataConclusao = DateTime.UtcNow;
        sol.AtualizadoEm = DateTime.UtcNow;
        _db.ResultadosExame.Add(resultado);
        await _db.SaveChangesAsync();
        return await _db.ResultadosExame.Include(r => r.RealizadoPor).FirstAsync(r => r.Id == resultado.Id);
    }

    public async Task<ResultadoExame?> ObterResultadoAsync(Guid solicitacaoId, Guid clinicaId)
    {
        var sol = await _db.SolicitacoesExame.FirstOrDefaultAsync(e => e.Id == solicitacaoId && e.ClinicaId == clinicaId);
        if (sol == null) return null;
        return await _db.ResultadosExame.Include(r => r.RealizadoPor).FirstOrDefaultAsync(r => r.SolicitacaoExameId == solicitacaoId);
    }
}
