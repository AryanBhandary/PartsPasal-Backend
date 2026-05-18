namespace PartsPasal.Application.Interfaces;

/// <summary>
/// Service for sending emails (Invoices, Reminders, Alerts).
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an invoice email (including an attached PDF(optional)) to the customer.
    /// </summary>
    Task SendInvoiceEmailAsync(string customerEmail, int invoiceId);

    /// <summary>
    /// Sends a reminder email for unpaid credits.
    /// </summary>
    Task SendCreditReminderEmailAsync(string customerEmail);
}
