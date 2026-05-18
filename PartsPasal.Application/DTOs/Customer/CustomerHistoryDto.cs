namespace PartsPasal.Application.DTOs.Customer;

public class CustomerHistoryDto
{
    public List<VehicleDto> Vehicles { get; set; } = new();
    public List<AppointmentDto> Appointments { get; set; } = new();
    public List<PartRequestDto> PartRequests { get; set; } = new();
    public List<SalesHistoryDto> Purchases { get; set; } = new();
}

public class SalesHistoryDto
{
    public int Id { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public bool IsPaid { get; set; }
    public List<SalesHistoryItemDto> Items { get; set; } = new();
}

public class SalesHistoryItemDto
{
    public string PartName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal SalePrice { get; set; }
}