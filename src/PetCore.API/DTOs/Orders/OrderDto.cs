namespace PetCore.API.DTOs.Orders;

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Period { get; set; }
    public string? Notes { get; set; }
    public string? Justification { get; set; }

    public Guid CreatedById { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public Guid? ApprovedById { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? ApprovedAt { get; set; }

    public DateTime CreatedAt { get; set; }
    public List<OrderItemDto> Items { get; set; } = [];
}

public class OrderItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string UnitAbbreviation { get; set; } = string.Empty;
    public int QuantityRequested { get; set; }
    public int? QuantityApproved { get; set; }
    public int QuantityReceived { get; set; }
    public string? Notes { get; set; }
}

public class CreateOrderRequest
{
    public string? Type { get; set; }
    public string? Period { get; set; }
    public string? Notes { get; set; }
    public string? Justification { get; set; }
    public List<CreateOrderItemRequest> Items { get; set; } = [];
}

public class CreateOrderItemRequest
{
    public Guid ProductId { get; set; }
    public int QuantityRequested { get; set; }
    public string? Notes { get; set; }
}

public class UpdateOrderRequest
{
    public string? Period { get; set; }
    public string? Notes { get; set; }
    public string? Justification { get; set; }
    public List<CreateOrderItemRequest>? Items { get; set; }
}

public class ReceiveOrderRequest
{
    public List<ReceiveOrderItemRequest> Items { get; set; } = [];
}

public class ReceiveOrderItemRequest
{
    public Guid OrderItemId { get; set; }
    public int QuantityReceived { get; set; }
}
