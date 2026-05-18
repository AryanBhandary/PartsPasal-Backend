namespace PartsPasal.Application.DTOs.Purchase;

public class PurchaseItemDto
{
    public int VehiclePartId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }
}