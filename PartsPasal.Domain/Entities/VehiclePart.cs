using System.ComponentModel.DataAnnotations;

namespace PartsPasal.Domain.Entities;

// Represents a vehicle part in the inventory
public class VehiclePart
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [Required]
    public decimal Price { get; set; }

    [Required]
    public int StockQuantity { get; set; }

    public int MinStockThreshold { get; set; } = 10;

    public int? VendorId { get; set; }

    // Navigation properties
    public Vendor? Vendor { get; set; }
    public ICollection<SalesInvoiceItem> SalesInvoiceItems { get; set; } = new List<SalesInvoiceItem>();
    public ICollection<PurchaseInvoiceItem> PurchaseInvoiceItems { get; set; } = new List<PurchaseInvoiceItem>();
}
