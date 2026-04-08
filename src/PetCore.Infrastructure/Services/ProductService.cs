using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class ProductService
{
    private readonly AppDbContext _db;

    public ProductService(AppDbContext db)
    {
        _db = db;
    }

    // --- Categories (globais) ---
    public async Task<List<ProductCategory>> GetCategoriesAsync() =>
        await _db.ProductCategories.OrderBy(c => c.Name).ToListAsync();

    public async Task<ProductCategory> CreateCategoryAsync(string name, string? description, string? color)
    {
        var cat = new ProductCategory
        {
            Id = Guid.NewGuid(), Name = name, Description = description,
            Color = color, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        };
        _db.ProductCategories.Add(cat);
        await _db.SaveChangesAsync();
        return cat;
    }

    public async Task<ProductCategory?> UpdateCategoryAsync(Guid id, string name, string? description, string? color)
    {
        var cat = await _db.ProductCategories.FindAsync(id);
        if (cat == null) return null;
        cat.Name = name; cat.Description = description; cat.Color = color;
        cat.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return cat;
    }

    // --- Units (globais) ---
    public async Task<List<ProductUnit>> GetUnitsAsync() =>
        await _db.ProductUnits.OrderBy(u => u.Name).ToListAsync();

    public async Task<ProductUnit> CreateUnitAsync(string abbreviation, string name)
    {
        var unit = new ProductUnit
        {
            Id = Guid.NewGuid(), Abbreviation = abbreviation, Name = name, CreatedAt = DateTime.UtcNow
        };
        _db.ProductUnits.Add(unit);
        await _db.SaveChangesAsync();
        return unit;
    }

    // --- Products ---
    private IQueryable<Product> BaseQuery(Guid clinicId) =>
        _db.Products
            .Include(p => p.Category)
            .Include(p => p.Unit)
            .Where(p => p.ClinicId == clinicId);

    public async Task<(List<Product> Items, int TotalCount)> GetPagedAsync(
        Guid clinicId, int page, int pageSize, string? search, Guid? categoryId, string? stockStatus)
    {
        var query = BaseQuery(clinicId);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search) || (p.Barcode != null && p.Barcode.Contains(search)));

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(stockStatus))
        {
            query = stockStatus.ToLower() switch
            {
                "zero" => query.Where(p => p.CurrentStock <= 0),
                "low" => query.Where(p => p.CurrentStock > 0 && p.CurrentStock <= p.MinStock),
                "ok" => query.Where(p => p.CurrentStock > p.MinStock),
                _ => query
            };
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Product?> GetByIdAsync(Guid id, Guid clinicId) =>
        await BaseQuery(clinicId).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Product> CreateAsync(Product product, Guid clinicId)
    {
        product.Id = Guid.NewGuid();
        product.ClinicId = clinicId;
        product.CreatedAt = DateTime.UtcNow;
        product.UpdatedAt = DateTime.UtcNow;
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return (await GetByIdAsync(product.Id, clinicId))!;
    }

    public async Task<Product?> UpdateAsync(Guid id, Guid clinicId, Action<Product> updateAction)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id && p.ClinicId == clinicId);
        if (product == null) return null;
        updateAction(product);
        product.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return await GetByIdAsync(id, clinicId);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid clinicId)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id && p.ClinicId == clinicId);
        if (product == null) return false;
        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<Product>> GetLowStockAsync(Guid clinicId) =>
        await BaseQuery(clinicId)
            .Where(p => p.Active && p.CurrentStock > 0 && p.CurrentStock <= p.MinStock)
            .OrderBy(p => p.CurrentStock)
            .ToListAsync();

    public async Task<List<Product>> GetZeroStockAsync(Guid clinicId) =>
        await BaseQuery(clinicId)
            .Where(p => p.Active && p.CurrentStock <= 0)
            .OrderBy(p => p.Name)
            .ToListAsync();

    public async Task<List<Product>> GetExpiringAsync(Guid clinicId, int daysAhead = 30) =>
        await BaseQuery(clinicId)
            .Where(p => p.Active && p.ExpirationDate != null &&
                        p.ExpirationDate <= DateTime.UtcNow.AddDays(daysAhead))
            .OrderBy(p => p.ExpirationDate)
            .ToListAsync();

    public async Task<List<Product>> GetAllForExportAsync(Guid clinicId, string? search, Guid? categoryId) =>
        await BaseQuery(clinicId)
            .Where(p => string.IsNullOrEmpty(search) || p.Name.Contains(search))
            .Where(p => !categoryId.HasValue || p.CategoryId == categoryId.Value)
            .OrderBy(p => p.Name)
            .ToListAsync();
}
