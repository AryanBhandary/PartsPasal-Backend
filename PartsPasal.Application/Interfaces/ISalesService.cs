using PartsPasal.Domain.Entities;

namespace PartsPasal.Application.Interfaces;

/// <summary>
/// Service for handling sales, invoicing, and loyalty discounts.
/// </summary>
public interface ISalesService
{
    // Task<SalesInvoice> CreateSalesInvoiceAsync(int customerId, List<LineItem> items);
    // Task ApplyLoyaltyDiscountAsync(SalesInvoice invoice); // 10% if > 5000
    // Task ProcessCreditPaymentAsync(int invoiceId, decimal amount);
}
