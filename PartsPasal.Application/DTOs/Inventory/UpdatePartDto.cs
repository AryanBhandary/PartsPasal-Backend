using System.ComponentModel.DataAnnotations;

namespace PartsPasal.Application.DTOs.Inventory;

public class UpdatePartDto
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative")]
    public decimal Price { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
    public int StockQuantity { get; set; }

    [Range(0, int.MaxValue)]
    public int MinStockThreshold { get; set; }
}