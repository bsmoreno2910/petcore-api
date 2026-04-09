using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class ServicoFinanceiro
{
    private readonly AppDbContext _db;

    public ServicoFinanceiro(AppDbContext db) { _db = db; }

    public async Task<List<CategoriaFinanceira>> ListarCategoriasAsync() =>
        await _db.CategoriasFinanceiras.OrderBy(c => c.Nome).ToListAsync();

    public async Task<CategoriaFinanceira> CriarCategoriaAsync(string nome, TipoTransacao tipo)
    {
        var cat = new CategoriaFinanceira { Id = Guid.NewGuid(), Nome = nome, Tipo = tipo, CriadoEm = DateTime.UtcNow };
        _db.CategoriasFinanceiras.Add(cat); await _db.SaveChangesAsync(); return cat;
    }

    private IQueryable<TransacaoFinanceira> QueryBase(Guid clinicaId) =>
        _db.TransacoesFinanceiras.Include(t => t.CategoriaFinanceira).Include(t => t.Tutor)
            .Include(t => t.CentroCusto).Include(t => t.CriadoPor).Include(t => t.Parcelas)
            .Where(t => t.ClinicaId == clinicaId);

    public async Task<(List<TransacaoFinanceira> Itens, int Total)> ListarPaginadoAsync(
        Guid clinicaId, int pagina, int tamanhoPagina,
        TipoTransacao? tipo, StatusTransacao? status, Guid? categoriaId, Guid? tutorId,
        DateTime? dataInicio, DateTime? dataFim)
    {
        var query = QueryBase(clinicaId);
        if (tipo.HasValue) query = query.Where(t => t.Tipo == tipo.Value);
        if (status.HasValue) query = query.Where(t => t.Status == status.Value);
        if (categoriaId.HasValue) query = query.Where(t => t.CategoriaFinanceiraId == categoriaId.Value);
        if (tutorId.HasValue) query = query.Where(t => t.TutorId == tutorId.Value);
        if (dataInicio.HasValue) query = query.Where(t => t.DataVencimento >= dataInicio.Value);
        if (dataFim.HasValue) query = query.Where(t => t.DataVencimento <= dataFim.Value);

        var total = await query.CountAsync();
        var itens = await query.OrderByDescending(t => t.DataVencimento).Skip((pagina - 1) * tamanhoPagina).Take(tamanhoPagina).ToListAsync();
        return (itens, total);
    }

    public async Task<TransacaoFinanceira?> ObterPorIdAsync(Guid id, Guid clinicaId) =>
        await QueryBase(clinicaId).FirstOrDefaultAsync(t => t.Id == id);

    public async Task<TransacaoFinanceira> CriarAsync(
        Guid clinicaId, Guid usuarioId, TipoTransacao tipo, Guid categoriaFinanceiraId,
        string descricao, decimal valor, decimal? desconto, MetodoPagamento? metodoPagamento,
        DateTime dataVencimento, Guid? tutorId, Guid? agendamentoId, Guid? internacaoId,
        Guid? solicitacaoExameId, Guid? centroCustoId, string? obs, string? numeroNota, int? parcelas)
    {
        var transacao = new TransacaoFinanceira
        {
            Id = Guid.NewGuid(), ClinicaId = clinicaId, Tipo = tipo, Status = StatusTransacao.Pendente,
            CategoriaFinanceiraId = categoriaFinanceiraId, Descricao = descricao, Valor = valor,
            Desconto = desconto, MetodoPagamento = metodoPagamento, DataVencimento = dataVencimento,
            TutorId = tutorId, AgendamentoId = agendamentoId, InternacaoId = internacaoId,
            SolicitacaoExameId = solicitacaoExameId, CentroCustoId = centroCustoId,
            Observacoes = obs, NumeroNota = numeroNota, CriadoPorId = usuarioId,
            CriadoEm = DateTime.UtcNow, AtualizadoEm = DateTime.UtcNow
        };
        _db.TransacoesFinanceiras.Add(transacao);

        if (parcelas.HasValue && parcelas.Value > 1)
        {
            var valorLiquido = valor - (desconto ?? 0);
            var valorParcela = Math.Round(valorLiquido / parcelas.Value, 2);
            for (int i = 1; i <= parcelas.Value; i++)
                _db.ParcelasTransacao.Add(new ParcelaTransacao
                {
                    Id = Guid.NewGuid(), TransacaoId = transacao.Id, NumeroParcela = i,
                    Valor = valorParcela, DataVencimento = dataVencimento.AddMonths(i - 1), Status = StatusTransacao.Pendente
                });
        }

        await _db.SaveChangesAsync();
        return (await ObterPorIdAsync(transacao.Id, clinicaId))!;
    }

    public async Task<TransacaoFinanceira?> PagarAsync(Guid id, Guid clinicaId, decimal? valorPago, MetodoPagamento? metodo)
    {
        var t = await _db.TransacoesFinanceiras.Include(x => x.Parcelas).FirstOrDefaultAsync(x => x.Id == id && x.ClinicaId == clinicaId);
        if (t == null) return null;
        var liquido = t.Valor - (t.Desconto ?? 0);
        t.ValorPago = valorPago ?? liquido;
        t.MetodoPagamento = metodo ?? t.MetodoPagamento;
        t.DataPagamento = DateTime.UtcNow;
        t.Status = t.ValorPago >= liquido ? StatusTransacao.Pago : StatusTransacao.ParcialmentePago;
        t.AtualizadoEm = DateTime.UtcNow;
        foreach (var p in t.Parcelas.Where(p => p.Status == StatusTransacao.Pendente))
        { p.DataPagamento = DateTime.UtcNow; p.Status = StatusTransacao.Pago; }
        await _db.SaveChangesAsync();
        return await ObterPorIdAsync(id, clinicaId);
    }

    public async Task<TransacaoFinanceira?> CancelarAsync(Guid id, Guid clinicaId)
    {
        var t = await _db.TransacoesFinanceiras.Include(x => x.Parcelas).FirstOrDefaultAsync(x => x.Id == id && x.ClinicaId == clinicaId);
        if (t == null) return null;
        t.Status = StatusTransacao.Cancelado; t.AtualizadoEm = DateTime.UtcNow;
        foreach (var p in t.Parcelas) p.Status = StatusTransacao.Cancelado;
        await _db.SaveChangesAsync();
        return await ObterPorIdAsync(id, clinicaId);
    }

    public async Task<ParcelaTransacao?> PagarParcelaAsync(Guid parcelaId)
    {
        var p = await _db.ParcelasTransacao.Include(i => i.Transacao).FirstOrDefaultAsync(i => i.Id == parcelaId);
        if (p == null) return null;
        p.DataPagamento = DateTime.UtcNow; p.Status = StatusTransacao.Pago;
        var todasPagas = await _db.ParcelasTransacao
            .Where(i => i.TransacaoId == p.TransacaoId && i.Id != parcelaId).AllAsync(i => i.Status == StatusTransacao.Pago);
        if (todasPagas) { p.Transacao.Status = StatusTransacao.Pago; p.Transacao.DataPagamento = DateTime.UtcNow; p.Transacao.ValorPago = p.Transacao.Valor - (p.Transacao.Desconto ?? 0); }
        else p.Transacao.Status = StatusTransacao.ParcialmentePago;
        p.Transacao.AtualizadoEm = DateTime.UtcNow;
        await _db.SaveChangesAsync(); return p;
    }

    public async Task<(decimal TotalReceita, decimal TotalDespesa, decimal TotalPendente, decimal TotalAtrasado, int Total)>
        ObterResumoAsync(Guid clinicaId)
    {
        var transacoes = await _db.TransacoesFinanceiras.Where(t => t.ClinicaId == clinicaId && t.Status != StatusTransacao.Cancelado).ToListAsync();
        return (
            transacoes.Where(t => t.Tipo == TipoTransacao.Receita && t.Status == StatusTransacao.Pago).Sum(t => t.ValorPago ?? t.Valor),
            transacoes.Where(t => t.Tipo == TipoTransacao.Despesa && t.Status == StatusTransacao.Pago).Sum(t => t.ValorPago ?? t.Valor),
            transacoes.Where(t => t.Status == StatusTransacao.Pendente).Sum(t => t.Valor),
            transacoes.Where(t => t.Status == StatusTransacao.Atrasado).Sum(t => t.Valor),
            transacoes.Count);
    }

    public async Task<List<TransacaoFinanceira>> ListarAtrasadasAsync(Guid clinicaId) =>
        await QueryBase(clinicaId).Where(t => t.Status == StatusTransacao.Pendente && t.DataVencimento < DateTime.UtcNow)
            .OrderBy(t => t.DataVencimento).ToListAsync();
}
