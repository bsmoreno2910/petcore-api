using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class OrderService
{
    private readonly AppDbContext _db;

    public OrderService(AppDbContext db)
    {
        _db = db;
    }

    private IQueryable<Order> BaseQuery(Guid clinicId) =>
        _db.Orders
            .Include(o => o.CreatedBy)
            .Include(o => o.ApprovedBy)
            .Include(o => o.Items).ThenInclude(i => i.Product).ThenInclude(p => p.Unit)
            .Where(o => o.ClinicId == clinicId);

    public async Task<List<Order>> GetAllAsync(Guid clinicId) =>
        await BaseQuery(clinicId).OrderByDescending(o => o.CreatedAt).ToListAsync();

    public async Task<Order?> GetByIdAsync(Guid id, Guid clinicId) =>
        await BaseQuery(clinicId).FirstOrDefaultAsync(o => o.Id == id);

    public async Task<Order> CreateAsync(
        Guid clinicId, Guid userId, OrderType type,
        string? period, string? notes, string? justification,
        List<(Guid ProductId, int QuantityRequested, string? Notes)> items)
    {
        var code = await GenerateCodeAsync(clinicId);

        var order = new Order
        {
            Id = Guid.NewGuid(),
            ClinicId = clinicId,
            Code = code,
            Type = type,
            Status = OrderStatus.Draft,
            Period = period,
            Notes = notes,
            Justification = justification,
            CreatedById = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        foreach (var item in items)
        {
            order.Items.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = item.ProductId,
                QuantityRequested = item.QuantityRequested,
                Notes = item.Notes
            });
        }

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        return (await GetByIdAsync(order.Id, clinicId))!;
    }

    public async Task<Order?> UpdateAsync(Guid id, Guid clinicId,
        string? period, string? notes, string? justification,
        List<(Guid ProductId, int QuantityRequested, string? Notes)>? items)
    {
        var order = await _db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id && o.ClinicId == clinicId);

        if (order == null) return null;

        if (order.Status != OrderStatus.Draft)
            throw new InvalidOperationException("Apenas pedidos em rascunho podem ser editados.");

        if (period != null) order.Period = period;
        if (notes != null) order.Notes = notes;
        if (justification != null) order.Justification = justification;

        if (items != null)
        {
            _db.OrderItems.RemoveRange(order.Items);
            order.Items.Clear();

            foreach (var item in items)
            {
                order.Items.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    QuantityRequested = item.QuantityRequested,
                    Notes = item.Notes
                });
            }
        }

        order.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return await GetByIdAsync(id, clinicId);
    }

    public async Task<Order?> SubmitAsync(Guid id, Guid clinicId)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == id && o.ClinicId == clinicId);
        if (order == null) return null;

        if (order.Status != OrderStatus.Draft)
            throw new InvalidOperationException("Apenas pedidos em rascunho podem ser submetidos.");

        order.Status = OrderStatus.Pending;
        order.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return await GetByIdAsync(id, clinicId);
    }

    public async Task<Order?> ApproveAsync(Guid id, Guid clinicId, Guid approverId)
    {
        var order = await _db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id && o.ClinicId == clinicId);

        if (order == null) return null;

        if (order.Status != OrderStatus.Pending)
            throw new InvalidOperationException("Apenas pedidos pendentes podem ser aprovados.");

        order.Status = OrderStatus.Approved;
        order.ApprovedById = approverId;
        order.ApprovedAt = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;

        // Por padrão, quantidade aprovada = quantidade solicitada
        foreach (var item in order.Items)
        {
            item.QuantityApproved ??= item.QuantityRequested;
        }

        await _db.SaveChangesAsync();
        return await GetByIdAsync(id, clinicId);
    }

    public async Task<Order?> ReceiveAsync(Guid id, Guid clinicId, Guid userId,
        List<(Guid OrderItemId, int QuantityReceived)> receivedItems)
    {
        var order = await _db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id && o.ClinicId == clinicId);

        if (order == null) return null;

        if (order.Status != OrderStatus.Approved && order.Status != OrderStatus.PartiallyReceived)
            throw new InvalidOperationException("Apenas pedidos aprovados podem ser recebidos.");

        foreach (var received in receivedItems)
        {
            var item = order.Items.FirstOrDefault(i => i.Id == received.OrderItemId)
                ?? throw new InvalidOperationException($"Item {received.OrderItemId} não encontrado no pedido.");

            item.QuantityReceived += received.QuantityReceived;

            // Criar movimentação de entrada
            var product = await _db.Products.FindAsync(item.ProductId)
                ?? throw new InvalidOperationException($"Produto não encontrado.");

            var previousStock = product.CurrentStock;
            product.CurrentStock += received.QuantityReceived;
            product.UpdatedAt = DateTime.UtcNow;

            _db.Movements.Add(new Movement
            {
                Id = Guid.NewGuid(),
                ClinicId = clinicId,
                ProductId = item.ProductId,
                Type = MovementType.Entry,
                Quantity = received.QuantityReceived,
                PreviousStock = previousStock,
                NewStock = product.CurrentStock,
                Reason = $"Recebimento pedido {order.Code}",
                CreatedById = userId,
                OrderId = order.Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        // Verificar se todos os itens foram totalmente recebidos
        var allReceived = order.Items.All(i =>
            i.QuantityReceived >= (i.QuantityApproved ?? i.QuantityRequested));

        order.Status = allReceived ? OrderStatus.Received : OrderStatus.PartiallyReceived;
        order.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return await GetByIdAsync(id, clinicId);
    }

    public async Task<Order?> CancelAsync(Guid id, Guid clinicId)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == id && o.ClinicId == clinicId);
        if (order == null) return null;

        if (order.Status == OrderStatus.Received || order.Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Pedido já finalizado ou cancelado.");

        order.Status = OrderStatus.Cancelled;
        order.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return await GetByIdAsync(id, clinicId);
    }

    private async Task<string> GenerateCodeAsync(Guid clinicId)
    {
        var count = await _db.Orders.CountAsync(o => o.ClinicId == clinicId);
        return $"PED-{count + 1:D5}";
    }
}
