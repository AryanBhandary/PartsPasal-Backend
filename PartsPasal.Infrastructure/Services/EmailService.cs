using PartsPasal.Application.Interfaces;

namespace PartsPasal.Infrastructure.Services;

/// <summary>
/// Email service for system notifications.
/// Required Feature: Email invoices, low stock alerts, and credit reminders.
/// </summary>
public class EmailService : IEmailService
{
    public Task SendInvoiceEmailAsync(string customerEmail, int invoiceId)
    {
        // Logic to generate PDF and send via SMTP
        return Task.CompletedTask;
    }

    public Task SendCreditReminderEmailAsync(string customerEmail)
    {
        // Logic to send reminder for unpaid credits > 1 month
        return Task.CompletedTask;
    }
}
