using System.ComponentModel.DataAnnotations;

namespace PartsPasal.Application.DTOs.Purchase;

public class CreatePurchaseItemDto
{
    [Required]
    public int VehiclePartId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Unit price cannot be negative")]
    public decimal UnitPrice { get; set; }
}