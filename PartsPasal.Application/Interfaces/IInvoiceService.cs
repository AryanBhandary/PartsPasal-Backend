using PartsPasal.Application.DTOs.Sales;

namespace PartsPasal.Application.Interfaces;

/// <summary>
/// Service for reading and emailing customer invoices.
/// </summary>
public interface IInvoiceService
{
    /// <summary>
    /// Gets a sales invoice by its id.
    /// </summary>
    Task<SalesInvoiceDto?> GetInvoiceByIdAsync(int id);

    /// <summary>
    /// Gets all sales invoices for a customer.
    /// </summary>
    Task<List<SalesInvoiceDto>> GetInvoicesByCustomerIdAsync(int customerId);

    /// <summary>
    /// Sends the invoice email to the customer through email.
    /// </summary>
    Task<bool> SendInvoiceEmailAsync(int invoiceId);
}