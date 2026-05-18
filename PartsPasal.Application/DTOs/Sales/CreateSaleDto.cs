namespace PartsPasal.Application.DTOs.Sales;

public class CreateSaleDto
{
    public int CustomerId { get; set; }
    public int StaffId { get; set; }

    /// <summary>
    /// Payment flag for the invoice.
    /// If null, the sale is treated as paid.
    /// Can be set to false to create a credit/unpaid invoice.
    /// </summary>
    public bool? IsPaid { get; set; }

    public List<CreateSaleItemDto> Items { get; set; } = new();
}

public class CreateSaleItemDto
{
    public int PartId { get; set; }
    public int Quantity { get; set; }
}