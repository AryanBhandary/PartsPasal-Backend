using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartsPasal.Domain.Entities;

// Represents a sales invoice for a customer purchase
public class SalesInvoice
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CustomerId { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public User Customer { get; set; } = null!;

    [Required]
    public int StaffId { get; set; }

    [ForeignKey(nameof(StaffId))]
    public User Staff { get; set; } = null!;

    public DateTime SaleDate { get; set; } = DateTime.UtcNow;

    // Sum of all line items before discount
    public decimal TotalAmount { get; set; }

    // Discount applied (10% if this invoice's TotalAmount exceeds 5000)
    public decimal DiscountAmount { get; set; }

    // TotalAmount minus DiscountAmount
    public decimal FinalAmount { get; set; }

    public bool IsPaid { get; set; }

    // Navigation properties
    public ICollection<SalesInvoiceItem> Items { get; set; } = new List<SalesInvoiceItem>();
}
