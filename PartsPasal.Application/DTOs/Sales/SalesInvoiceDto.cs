namespace PartsPasal.Application.DTOs.Sales;

public class SalesInvoiceDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int StaffId { get; set; }

    public DateTime SaleDate { get; set; }

    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }

    public bool IsPaid { get; set; }

    public List<SalesInvoiceItemDto> Items { get; set; } = new();
}

public class SalesInvoiceItemDto
{
    public int PartId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal SalePrice { get; set; }
}