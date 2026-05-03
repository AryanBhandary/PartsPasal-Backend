using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using PartsPasal.Application.Interfaces;
using PartsPasal.Infrastructure.Data;

namespace PartsPasal.Infrastructure.Services;

/// <summary>
/// Email service for system notifications such as due credits.
/// Sends emails via SMTP.
/// </summary>
public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtp;
    private readonly AppDbContext _db;

    public EmailService(IOptions<SmtpSettings> smtpOptions, AppDbContext db)
    {
        _smtp = smtpOptions.Value;
        _db = db;

        if (string.IsNullOrWhiteSpace(_smtp.Host))
            throw new InvalidOperationException("SMTP Host is missing (Smtp:Host).");

        if (_smtp.Port <= 0)
            throw new InvalidOperationException("SMTP Port is missing/invalid (Smtp:Port).");

        if (string.IsNullOrWhiteSpace(_smtp.Username))
            throw new InvalidOperationException("SMTP Username is missing (Smtp:Username).");

        if (string.IsNullOrWhiteSpace(_smtp.Password))
            throw new InvalidOperationException("SMTP Password is missing (Smtp:Password).");

        if (string.IsNullOrWhiteSpace(_smtp.FromEmail))
            throw new InvalidOperationException("SMTP FromEmail is missing (Smtp:FromEmail).");
    }

    public async Task SendInvoiceEmailAsync(string customerEmail, int invoiceId)
    {
        var invoice = await _db.SalesInvoices
            .AsNoTracking()
            .Include(i => i.Customer)
            .Include(i => i.Staff)
            .Include(i => i.Items)
            .ThenInclude(ii => ii.VehiclePartId)
            .FirstOrDefaultAsync(i => i.Id == invoiceId);

        if (invoice == null)
            throw new InvalidOperationException($"Invoice not found (id={invoiceId}).");

        var subject = $"PartsPasal Invoice #{invoice.Id}";

        var customerName = invoice.Customer?.Name ?? "Customer";
        var staffName = invoice.Staff?.Name ?? "";

        var lines = new List<string>
        {
            $"Hello {customerName},",
            "",
            "Thank you for your purchase. Here are your invoice details:",
            "",
            $"Invoice ID: {invoice.Id}",
            $"Date (UTC): {invoice.SaleDate:yyyy-MM-dd HH:mm}",
            $"Total: {invoice.TotalAmount:0.00}",
            $"Discount: {invoice.DiscountAmount:0.00}",
            $"Final: {invoice.FinalAmount:0.00}",
            $"Paid: {(invoice.IsPaid ? "Yes" : "No")}",
            "",
            "Items:",
        };

        if (!string.IsNullOrWhiteSpace(staffName))
            lines.Insert(6, $"Handled by: {staffName}");

        foreach (var item in invoice.Items.OrderBy(i => i.Id))
        {
            var partName = item.VehiclePart?.Name ?? $"Part #{item.VehiclePartId}";
            var lineTotal = item.SalePrice * item.Quantity;
            lines.Add($"- {partName} | Qty: {item.Quantity} | Unit: {item.SalePrice:0.00} | Line: {lineTotal:0.00}");
        }

        lines.AddRange(new[]
        {
            "",
            "If you have any questions, please reply to this email.",
            "",
            "PartsPasal"
        });

        var body = string.Join("\n", lines);

        await SendEmailAsync(customerEmail, subject, body);
    }

    public Task SendCreditReminderEmailAsync(string customerEmail)
    {
        var subject = "Payment Reminder";
        var body = "You have an unpaid credit balance overdue by more than one month. Please clear it as soon as possible.";

        return SendEmailAsync(customerEmail, subject, body);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        if (string.IsNullOrWhiteSpace(toEmail))
            throw new ArgumentException("Recipient email is required.", nameof(toEmail));

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_smtp.FromName, _smtp.FromEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;

        message.Body = new TextPart("plain")
        {
            Text = body
        };

        using var client = new SmtpClient();

        // Gmail SMTP
        await client.ConnectAsync(_smtp.Host, _smtp.Port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_smtp.Username, _smtp.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}