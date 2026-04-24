using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PartsPasal.Domain.Enums;

namespace PartsPasal.Domain.Entities;

// Represents an invoice for purchasing stock from a vendor
public class PurchaseInvoice
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int VendorId { get; set; }

    [ForeignKey(nameof(VendorId))]
    public Vendor Vendor { get; set; } = null!;

    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

    public decimal TotalAmount { get; set; }

    [Required]
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;

    // Navigation properties
    public ICollection<PurchaseInvoiceItem> Items { get; set; } = new List<PurchaseInvoiceItem>();
}
