namespace PartsPasal.Application.Interfaces;

/// <summary>
/// Service for sending emails (Invoices, Reminders, Alerts).
/// </summary>
public interface IEmailService
{
    // Task SendInvoiceEmailAsync(string customerEmail, int invoiceId);
    // Task SendCreditReminderEmailAsync(string customerEmail); // For > 1 month unpaid
}
