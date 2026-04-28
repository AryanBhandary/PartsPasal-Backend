namespace PartsPasal.Application.DTOs.Invoices;

/// <summary>
/// Request DTO for emailing an invoice.
/// </summary>
public class SendInvoiceEmailDto
{
    public int InvoiceId { get; set; }
}