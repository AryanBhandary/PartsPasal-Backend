namespace PartsPasal.Application.DTOs.Purchase;

public class PurchaseDto
{
    public int Id { get; set; }

    public int VendorId { get; set; }

    public DateTime PurchaseDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = string.Empty;

    public List<PurchaseItemDto> Items { get; set; } = new();
}