using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartsPasal.Domain.Entities;

// Represents a line item within a sales invoice
public class SalesInvoiceItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int SalesInvoiceId { get; set; }

    [ForeignKey(nameof(SalesInvoiceId))]
    public SalesInvoice SalesInvoice { get; set; } = null!;

    [Required]
    public int VehiclePartId { get; set; }

    [ForeignKey(nameof(VehiclePartId))]
    public VehiclePart VehiclePart { get; set; } = null!;

    [Required]
    public int Quantity { get; set; }

    // Price per unit at the time of sale
    [Required]
    public decimal SalePrice { get; set; }
}
