using System.ComponentModel.DataAnnotations;

namespace PartsPasal.Application.DTOs.Purchase;

public class CreatePurchaseDto
{
    [Required]
    public int VendorId { get; set; }

    [Required]
    public List<CreatePurchaseItemDto> Items { get; set; } = new();
}