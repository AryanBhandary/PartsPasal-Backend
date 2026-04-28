namespace PartsPasal.Application.DTOs.Sales;

public class CreateSaleDto
{
    public int CustomerId { get; set; }
    public int StaffId { get; set; }

    public List<CreateSaleItemDto> Items { get; set; } = new();
}

public class CreateSaleItemDto
{
    public int PartId { get; set; }
    public int Quantity { get; set; }
}