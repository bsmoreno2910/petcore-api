using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class ServicoProduto
{
    private readonly AppDbContext _db;

    public ServicoProduto(AppDbContext db) { _db = db; }

    public async Task<List<CategoriaProduto>> ListarCategoriasAsync() => await _db.CategoriasProduto.OrderBy(c => c.Nome).ToListAsync();
    public async Task<CategoriaProduto> CriarCategoriaAsync(string nome, string? descricao, string? cor)
    {
        var cat = new CategoriaProduto { Id = Guid.NewGuid(), Nome = nome, Descricao = descricao, Cor = cor, CriadoEm = DateTime.UtcNow, AtualizadoEm = DateTime.UtcNow };
        _db.CategoriasProduto.Add(cat); await _db.SaveChangesAsync(); return cat;
    }
    public async Task<CategoriaProduto?> AtualizarCategoriaAsync(Guid id, string nome, string? descricao, string? cor)
    {
        var cat = await _db.CategoriasProduto.FindAsync(id); if (cat == null) return null;
        cat.Nome = nome; cat.Descricao = descricao; cat.Cor = cor; cat.AtualizadoEm = DateTime.UtcNow;
        await _db.SaveChangesAsync(); return cat;
    }

    public async Task<List<UnidadeProduto>> ListarUnidadesAsync() => await _db.UnidadesProduto.OrderBy(u => u.Nome).ToListAsync();
    public async Task<UnidadeProduto> CriarUnidadeAsync(string sigla, string nome)
    {
        var u = new UnidadeProduto { Id = Guid.NewGuid(), Sigla = sigla, Nome = nome, CriadoEm = DateTime.UtcNow };
        _db.UnidadesProduto.Add(u); await _db.SaveChangesAsync(); return u;
    }

    private IQueryable<Produto> QueryBase(Guid clinicaId) =>
        _db.Produtos.Include(p => p.Categoria).Include(p => p.Unidade).Where(p => p.ClinicaId == clinicaId);

    public async Task<(List<Produto> Itens, int Total)> ListarPaginadoAsync(
        Guid clinicaId, int pagina, int tamanhoPagina, string? busca, Guid? categoriaId, string? statusEstoque)
    {
        var query = QueryBase(clinicaId);
        if (!string.IsNullOrWhiteSpace(busca)) query = query.Where(p => p.Nome.Contains(busca) || (p.CodigoBarras != null && p.CodigoBarras.Contains(busca)));
        if (categoriaId.HasValue) query = query.Where(p => p.CategoriaId == categoriaId.Value);
        if (!string.IsNullOrWhiteSpace(statusEstoque))
            query = statusEstoque.ToLower() switch
            {
                "zerado" => query.Where(p => p.EstoqueAtual <= 0),
                "baixo" => query.Where(p => p.EstoqueAtual > 0 && p.EstoqueAtual <= p.EstoqueMinimo),
                "ok" => query.Where(p => p.EstoqueAtual > p.EstoqueMinimo),
                _ => query
            };
        var total = await query.CountAsync();
        var itens = await query.OrderBy(p => p.Nome).Skip((pagina - 1) * tamanhoPagina).Take(tamanhoPagina).ToListAsync();
        return (itens, total);
    }

    public async Task<Produto?> ObterPorIdAsync(Guid id, Guid clinicaId) => await QueryBase(clinicaId).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Produto> CriarAsync(Produto produto, Guid clinicaId)
    {
        produto.Id = Guid.NewGuid(); produto.ClinicaId = clinicaId;
        produto.CriadoEm = DateTime.UtcNow; produto.AtualizadoEm = DateTime.UtcNow;
        _db.Produtos.Add(produto); await _db.SaveChangesAsync();
        return (await ObterPorIdAsync(produto.Id, clinicaId))!;
    }

    public async Task<Produto?> AtualizarAsync(Guid id, Guid clinicaId, Action<Produto> acao)
    {
        var p = await _db.Produtos.FirstOrDefaultAsync(x => x.Id == id && x.ClinicaId == clinicaId);
        if (p == null) return null; acao(p); p.AtualizadoEm = DateTime.UtcNow;
        await _db.SaveChangesAsync(); return await ObterPorIdAsync(id, clinicaId);
    }

    public async Task<bool> ExcluirAsync(Guid id, Guid clinicaId)
    {
        var p = await _db.Produtos.FirstOrDefaultAsync(x => x.Id == id && x.ClinicaId == clinicaId);
        if (p == null) return false; _db.Produtos.Remove(p); await _db.SaveChangesAsync(); return true;
    }

    public async Task<List<Produto>> ListarEstoqueBaixoAsync(Guid clinicaId) =>
        await QueryBase(clinicaId).Where(p => p.Ativo && p.EstoqueAtual > 0 && p.EstoqueAtual <= p.EstoqueMinimo).OrderBy(p => p.EstoqueAtual).ToListAsync();

    public async Task<List<Produto>> ListarEstoqueZeradoAsync(Guid clinicaId) =>
        await QueryBase(clinicaId).Where(p => p.Ativo && p.EstoqueAtual <= 0).OrderBy(p => p.Nome).ToListAsync();

    public async Task<List<Produto>> ListarVencimentoProximoAsync(Guid clinicaId, int dias = 30) =>
        await QueryBase(clinicaId).Where(p => p.Ativo && p.DataValidade != null && p.DataValidade <= DateTime.UtcNow.AddDays(dias))
            .OrderBy(p => p.DataValidade).ToListAsync();
}
