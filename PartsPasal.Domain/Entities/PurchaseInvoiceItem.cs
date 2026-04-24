using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartsPasal.Domain.Entities;

// Represents a line item within a purchase invoice
public class PurchaseInvoiceItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int PurchaseInvoiceId { get; set; }

    [ForeignKey(nameof(PurchaseInvoiceId))]
    public PurchaseInvoice PurchaseInvoice { get; set; } = null!;

    [Required]
    public int VehiclePartId { get; set; }

    [ForeignKey(nameof(VehiclePartId))]
    public VehiclePart VehiclePart { get; set; } = null!;

    [Required]
    public int Quantity { get; set; }

    // Price per unit at the time of purchase
    [Required]
    public decimal UnitPrice { get; set; }
}
