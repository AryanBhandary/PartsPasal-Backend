using System.ComponentModel.DataAnnotations;

namespace PartsPasal.Domain.Entities;

/// <summary>
/// Represents an invoice for purchasing stock from a vendor.
/// Required Feature: Admin creates purchase invoices to update stock.
/// </summary>
public class PurchaseInvoice
{
    // Id, VendorId, PurchaseDate, TotalAmount, Status (Pending/Completed)
    // List of LineItems (PartId, Quantity, UnitPrice)
}
