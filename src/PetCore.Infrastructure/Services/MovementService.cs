using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class MovementService
{
    private readonly AppDbContext _db;

    public MovementService(AppDbContext db)
    {
        _db = db;
    }

    private IQueryable<Movement> BaseQuery(Guid clinicId) =>
        _db.Movements
            .Include(m => m.Product)
            .Include(m => m.CreatedBy)
            .Include(m => m.ApprovedBy)
            .Where(m => m.ClinicId == clinicId);

    public async Task<(List<Movement> Items, int TotalCount)> GetPagedAsync(
        Guid clinicId, int page, int pageSize,
        MovementType? type, Guid? productId, Guid? userId,
        DateTime? startDate, DateTime? endDate)
    {
        var query = BaseQuery(clinicId);

        if (type.HasValue) query = query.Where(m => m.Type == type.Value);
        if (productId.HasValue) query = query.Where(m => m.ProductId == productId.Value);
        if (userId.HasValue) query = query.Where(m => m.CreatedById == userId.Value);
        if (startDate.HasValue) query = query.Where(m => m.CreatedAt >= startDate.Value);
        if (endDate.HasValue) query = query.Where(m => m.CreatedAt <= endDate.Value);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Movement?> GetByIdAsync(Guid id, Guid clinicId) =>
        await BaseQuery(clinicId).FirstOrDefaultAsync(m => m.Id == id);

    public async Task<Movement> CreateEntryAsync(Guid clinicId, Guid userId, Guid productId, int quantity, string? reason, string? notes)
        => await CreateMovementAsync(clinicId, userId, productId, MovementType.Entry, quantity, reason, notes);

    public async Task<Movement> CreateExitAsync(Guid clinicId, Guid userId, Guid productId, int quantity, string? reason, string? notes)
        => await CreateMovementAsync(clinicId, userId, productId, MovementType.Exit, quantity, reason, notes);

    public async Task<Movement> CreateAdjustmentAsync(Guid clinicId, Guid userId, Guid productId, int quantity, string? reason, string? notes)
        => await CreateMovementAsync(clinicId, userId, productId, MovementType.Adjustment, quantity, reason, notes);

    public async Task<Movement> CreateLossAsync(Guid clinicId, Guid userId, Guid productId, int quantity, string? reason, string? notes)
        => await CreateMovementAsync(clinicId, userId, productId, MovementType.Loss, quantity, reason, notes);

    private async Task<Movement> CreateMovementAsync(
        Guid clinicId, Guid userId, Guid productId,
        MovementType type, int quantity, string? reason, string? notes)
    {
        if (quantity <= 0)
            throw new InvalidOperationException("Quantidade deve ser maior que zero.");

        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == productId && p.ClinicId == clinicId)
            ?? throw new InvalidOperationException("Produto não encontrado.");

        var previousStock = product.CurrentStock;
        var newStock = type switch
        {
            MovementType.Entry => previousStock + quantity,
            MovementType.Exit => previousStock - quantity,
            MovementType.Adjustment => quantity, // ajuste define o estoque diretamente
            MovementType.Loss => previousStock - quantity,
            MovementType.Return => previousStock + quantity,
            _ => previousStock
        };

        if (newStock < 0)
            throw new InvalidOperationException($"Estoque insuficiente. Estoque atual: {previousStock}, solicitado: {quantity}.");

        product.CurrentStock = newStock;
        product.UpdatedAt = DateTime.UtcNow;

        var movement = new Movement
        {
            Id = Guid.NewGuid(),
            ClinicId = clinicId,
            ProductId = productId,
            Type = type,
            Quantity = quantity,
            PreviousStock = previousStock,
            NewStock = newStock,
            Reason = reason,
            Notes = notes,
            CreatedById = userId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Movements.Add(movement);
        await _db.SaveChangesAsync();

        return (await GetByIdAsync(movement.Id, clinicId))!;
    }
}
