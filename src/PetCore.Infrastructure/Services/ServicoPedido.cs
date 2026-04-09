using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class ServicoPedido
{
    private readonly AppDbContext _db;

    public ServicoPedido(AppDbContext db) { _db = db; }

    private IQueryable<Pedido> QueryBase(Guid clinicaId) =>
        _db.Pedidos.Include(o => o.CriadoPor).Include(o => o.AprovadoPor)
            .Include(o => o.Itens).ThenInclude(i => i.Produto).ThenInclude(p => p.Unidade)
            .Where(o => o.ClinicaId == clinicaId);

    public async Task<List<Pedido>> ListarTodosAsync(Guid clinicaId) =>
        await QueryBase(clinicaId).OrderByDescending(o => o.CriadoEm).ToListAsync();

    public async Task<Pedido?> ObterPorIdAsync(Guid id, Guid clinicaId) =>
        await QueryBase(clinicaId).FirstOrDefaultAsync(o => o.Id == id);

    public async Task<Pedido> CriarAsync(Guid clinicaId, Guid usuarioId, TipoPedido tipo,
        string? periodo, string? obs, string? justificativa,
        List<(Guid ProdutoId, int QtdSolicitada, string? Obs)> itens)
    {
        var codigo = $"PED-{await _db.Pedidos.CountAsync(o => o.ClinicaId == clinicaId) + 1:D5}";
        var pedido = new Pedido
        {
            Id = Guid.NewGuid(), ClinicaId = clinicaId, Codigo = codigo, Tipo = tipo,
            Status = StatusPedido.Rascunho, Periodo = periodo, Observacoes = obs,
            Justificativa = justificativa, CriadoPorId = usuarioId,
            CriadoEm = DateTime.UtcNow, AtualizadoEm = DateTime.UtcNow
        };
        foreach (var item in itens)
            pedido.Itens.Add(new ItemPedido { Id = Guid.NewGuid(), ProdutoId = item.ProdutoId, QuantidadeSolicitada = item.QtdSolicitada, Observacoes = item.Obs });
        _db.Pedidos.Add(pedido);
        await _db.SaveChangesAsync();
        return (await ObterPorIdAsync(pedido.Id, clinicaId))!;
    }

    public async Task<Pedido?> EnviarAsync(Guid id, Guid clinicaId)
    {
        var p = await _db.Pedidos.FirstOrDefaultAsync(o => o.Id == id && o.ClinicaId == clinicaId);
        if (p == null) return null;
        if (p.Status != StatusPedido.Rascunho) throw new InvalidOperationException("Apenas rascunhos podem ser enviados.");
        p.Status = StatusPedido.Pendente; p.AtualizadoEm = DateTime.UtcNow;
        await _db.SaveChangesAsync(); return await ObterPorIdAsync(id, clinicaId);
    }

    public async Task<Pedido?> AprovarAsync(Guid id, Guid clinicaId, Guid aprovadorId)
    {
        var p = await _db.Pedidos.Include(o => o.Itens).FirstOrDefaultAsync(o => o.Id == id && o.ClinicaId == clinicaId);
        if (p == null) return null;
        if (p.Status != StatusPedido.Pendente) throw new InvalidOperationException("Apenas pedidos pendentes podem ser aprovados.");
        p.Status = StatusPedido.Aprovado; p.AprovadoPorId = aprovadorId; p.DataAprovacao = DateTime.UtcNow; p.AtualizadoEm = DateTime.UtcNow;
        foreach (var item in p.Itens) item.QuantidadeAprovada ??= item.QuantidadeSolicitada;
        await _db.SaveChangesAsync(); return await ObterPorIdAsync(id, clinicaId);
    }

    public async Task<Pedido?> ReceberAsync(Guid id, Guid clinicaId, Guid usuarioId,
        List<(Guid ItemId, int QtdRecebida)> itensRecebidos)
    {
        var p = await _db.Pedidos.Include(o => o.Itens).FirstOrDefaultAsync(o => o.Id == id && o.ClinicaId == clinicaId);
        if (p == null) return null;
        if (p.Status != StatusPedido.Aprovado && p.Status != StatusPedido.ParcialmenteRecebido)
            throw new InvalidOperationException("Apenas pedidos aprovados podem ser recebidos.");

        foreach (var recebido in itensRecebidos)
        {
            var item = p.Itens.FirstOrDefault(i => i.Id == recebido.ItemId)
                ?? throw new InvalidOperationException($"Item {recebido.ItemId} não encontrado.");
            item.QuantidadeRecebida += recebido.QtdRecebida;
            var produto = await _db.Produtos.FindAsync(item.ProdutoId) ?? throw new InvalidOperationException("Produto não encontrado.");
            var anterior = produto.EstoqueAtual;
            produto.EstoqueAtual += recebido.QtdRecebida;
            produto.AtualizadoEm = DateTime.UtcNow;
            _db.Movimentacoes.Add(new Movimentacao
            {
                Id = Guid.NewGuid(), ClinicaId = clinicaId, ProdutoId = item.ProdutoId,
                Tipo = TipoMovimentacao.Entrada, Quantidade = recebido.QtdRecebida,
                EstoqueAnterior = anterior, NovoEstoque = produto.EstoqueAtual,
                Motivo = $"Recebimento pedido {p.Codigo}", CriadoPorId = usuarioId,
                PedidoId = p.Id, CriadoEm = DateTime.UtcNow
            });
        }

        p.Status = p.Itens.All(i => i.QuantidadeRecebida >= (i.QuantidadeAprovada ?? i.QuantidadeSolicitada))
            ? StatusPedido.Recebido : StatusPedido.ParcialmenteRecebido;
        p.AtualizadoEm = DateTime.UtcNow;
        await _db.SaveChangesAsync(); return await ObterPorIdAsync(id, clinicaId);
    }

    public async Task<Pedido?> CancelarAsync(Guid id, Guid clinicaId)
    {
        var p = await _db.Pedidos.FirstOrDefaultAsync(o => o.Id == id && o.ClinicaId == clinicaId);
        if (p == null) return null;
        if (p.Status == StatusPedido.Recebido || p.Status == StatusPedido.Cancelado)
            throw new InvalidOperationException("Pedido já finalizado ou cancelado.");
        p.Status = StatusPedido.Cancelado; p.AtualizadoEm = DateTime.UtcNow;
        await _db.SaveChangesAsync(); return await ObterPorIdAsync(id, clinicaId);
    }
}
