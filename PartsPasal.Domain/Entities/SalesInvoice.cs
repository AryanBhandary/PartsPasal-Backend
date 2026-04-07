using System.ComponentModel.DataAnnotations;

namespace PartsPasal.Domain.Entities;

/// <summary>
/// Represents a sales invoice for a customer purchase.
/// Required Feature: Staff handles part sales and creates invoices.
/// Automatically applies 10% discount if total > 5000.
/// </summary>
public class SalesInvoice
{
    // Id, CustomerId, StaffId, SaleDate, TotalAmount, DiscountAmount, FinalAmount, IsPaid
    // List of LineItems (PartId, Quantity, SalePrice)
}
