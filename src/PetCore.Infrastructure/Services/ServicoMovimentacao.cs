using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class ServicoMovimentacao
{
    private readonly AppDbContext _db;

    public ServicoMovimentacao(AppDbContext db) { _db = db; }

    private IQueryable<Movimentacao> QueryBase(Guid clinicaId) =>
        _db.Movimentacoes.Include(m => m.Produto).Include(m => m.CriadoPor).Include(m => m.AprovadoPor)
            .Where(m => m.ClinicaId == clinicaId);

    public async Task<(List<Movimentacao> Itens, int Total)> ListarPaginadoAsync(
        Guid clinicaId, int pagina, int tamanhoPagina,
        TipoMovimentacao? tipo, Guid? produtoId, Guid? usuarioId, DateTime? dataInicio, DateTime? dataFim)
    {
        var query = QueryBase(clinicaId);
        if (tipo.HasValue) query = query.Where(m => m.Tipo == tipo.Value);
        if (produtoId.HasValue) query = query.Where(m => m.ProdutoId == produtoId.Value);
        if (usuarioId.HasValue) query = query.Where(m => m.CriadoPorId == usuarioId.Value);
        if (dataInicio.HasValue) query = query.Where(m => m.CriadoEm >= dataInicio.Value);
        if (dataFim.HasValue) query = query.Where(m => m.CriadoEm <= dataFim.Value);

        var total = await query.CountAsync();
        var itens = await query.OrderByDescending(m => m.CriadoEm).Skip((pagina - 1) * tamanhoPagina).Take(tamanhoPagina).ToListAsync();
        return (itens, total);
    }

    public async Task<Movimentacao?> ObterPorIdAsync(Guid id, Guid clinicaId) =>
        await QueryBase(clinicaId).FirstOrDefaultAsync(m => m.Id == id);

    public async Task<Movimentacao> CriarEntradaAsync(Guid clinicaId, Guid usuarioId, Guid produtoId, int qtd, string? motivo, string? obs)
        => await CriarMovimentacaoAsync(clinicaId, usuarioId, produtoId, TipoMovimentacao.Entrada, qtd, motivo, obs);
    public async Task<Movimentacao> CriarSaidaAsync(Guid clinicaId, Guid usuarioId, Guid produtoId, int qtd, string? motivo, string? obs)
        => await CriarMovimentacaoAsync(clinicaId, usuarioId, produtoId, TipoMovimentacao.Saida, qtd, motivo, obs);
    public async Task<Movimentacao> CriarAjusteAsync(Guid clinicaId, Guid usuarioId, Guid produtoId, int qtd, string? motivo, string? obs)
        => await CriarMovimentacaoAsync(clinicaId, usuarioId, produtoId, TipoMovimentacao.Ajuste, qtd, motivo, obs);
    public async Task<Movimentacao> CriarPerdaAsync(Guid clinicaId, Guid usuarioId, Guid produtoId, int qtd, string? motivo, string? obs)
        => await CriarMovimentacaoAsync(clinicaId, usuarioId, produtoId, TipoMovimentacao.Perda, qtd, motivo, obs);

    private async Task<Movimentacao> CriarMovimentacaoAsync(
        Guid clinicaId, Guid usuarioId, Guid produtoId, TipoMovimentacao tipo, int qtd, string? motivo, string? obs)
    {
        if (qtd <= 0) throw new InvalidOperationException("Quantidade deve ser maior que zero.");
        var produto = await _db.Produtos.FirstOrDefaultAsync(p => p.Id == produtoId && p.ClinicaId == clinicaId)
            ?? throw new InvalidOperationException("Produto não encontrado.");

        var estoqueAnterior = produto.EstoqueAtual;
        var novoEstoque = tipo switch
        {
            TipoMovimentacao.Entrada => estoqueAnterior + qtd,
            TipoMovimentacao.Saida => estoqueAnterior - qtd,
            TipoMovimentacao.Ajuste => qtd,
            TipoMovimentacao.Perda => estoqueAnterior - qtd,
            TipoMovimentacao.Retorno => estoqueAnterior + qtd,
            _ => estoqueAnterior
        };

        if (novoEstoque < 0)
            throw new InvalidOperationException($"Estoque insuficiente. Atual: {estoqueAnterior}, solicitado: {qtd}.");

        produto.EstoqueAtual = novoEstoque;
        produto.AtualizadoEm = DateTime.UtcNow;

        var mov = new Movimentacao
        {
            Id = Guid.NewGuid(), ClinicaId = clinicaId, ProdutoId = produtoId, Tipo = tipo,
            Quantidade = qtd, EstoqueAnterior = estoqueAnterior, NovoEstoque = novoEstoque,
            Motivo = motivo, Observacoes = obs, CriadoPorId = usuarioId, CriadoEm = DateTime.UtcNow
        };
        _db.Movimentacoes.Add(mov);
        await _db.SaveChangesAsync();
        return (await ObterPorIdAsync(mov.Id, clinicaId))!;
    }
}
