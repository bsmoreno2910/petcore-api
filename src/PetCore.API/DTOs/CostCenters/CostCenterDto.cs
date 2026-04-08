namespace PetCore.API.DTOs.CostCenters;

public class CostCenterDto
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCostCenterRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateCostCenterRequest : CreateCostCenterRequest { }

public class CostCenterSummaryDto
{
    public Guid CostCenterId { get; set; }
    public string CostCenterName { get; set; } = string.Empty;
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal Balance { get; set; }
    public int TransactionCount { get; set; }
}
