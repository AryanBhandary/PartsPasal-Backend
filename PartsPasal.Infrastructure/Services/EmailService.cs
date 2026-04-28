using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using PartsPasal.Application.Interfaces;

namespace PartsPasal.Infrastructure.Services;

/// <summary>
/// Email service for system notifications such as due credits.
/// Sends emails via SMTP.
/// </summary>
public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtp;

    public EmailService(IOptions<SmtpSettings> smtpOptions)
    {
        _smtp = smtpOptions.Value;

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

    public Task SendInvoiceEmailAsync(string customerEmail, int invoiceId)
    {
        // Subject and body
        var subject = $"Invoice #{invoiceId}";
        var body = $"Thank you for your purchase. Your invoice id is #{invoiceId}.";

        return SendEmailAsync(customerEmail, subject, body);
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