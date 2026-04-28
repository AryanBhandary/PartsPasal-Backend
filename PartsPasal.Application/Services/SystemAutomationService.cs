using PartsPasal.Application.DTOs.Notifications;
using PartsPasal.Application.DTOs.System;
using PartsPasal.Application.Interfaces;
using PartsPasal.Domain.Entities;

namespace PartsPasal.Application.Services;

/// <summary>
/// Implements internal system checks that can be triggered via internal APIs or scheduled jobs.
/// </summary>
public class SystemAutomationService : ISystemAutomationService
{
    private readonly IRepositoryBase<VehiclePart> _partRepo;
    private readonly IRepositoryBase<SalesInvoice> _salesInvoiceRepo;
    private readonly IRepositoryBase<User> _userRepo;
    private readonly IEmailService _emailService;
    private readonly INotificationService _notificationService;

    public SystemAutomationService(
        IRepositoryBase<VehiclePart> partRepo,
        IRepositoryBase<SalesInvoice> salesInvoiceRepo,
        IRepositoryBase<User> userRepo,
        IEmailService emailService,
        INotificationService notificationService)
    {
        _partRepo = partRepo;
        _salesInvoiceRepo = salesInvoiceRepo;
        _userRepo = userRepo;
        _emailService = emailService;
        _notificationService = notificationService;
    }

    public async Task<LowStockCheckResultDto> CheckLowStockAsync()
    {
        // notify admin when stock falls below the threshold (default threshold = 10).
        var lowStockParts = await _partRepo.FindAsync(p => p.StockQuantity < p.MinStockThreshold);

        var result = new LowStockCheckResultDto
        {
            Count = lowStockParts.Count,
            Parts = lowStockParts
                .OrderBy(p => p.StockQuantity)
                .Select(p => new LowStockPartDto
                {
                    PartId = p.Id,
                    Name = p.Name,
                    StockQuantity = p.StockQuantity,
                    MinStockThreshold = p.MinStockThreshold
                })
                .ToList()
        };

        // Storing a broadcast(to all users) notification so Admin dashboards can show the alert.
        if (result.Count > 0)
        {
            var previewParts = result.Parts.Take(20).ToList();
            var remaining = result.Parts.Count - previewParts.Count;

            var partsText = string.Join(", ", previewParts.Select(p => $"{p.Name}({p.StockQuantity})"));
            if (remaining > 0)
                partsText += $" and {remaining} more";

            await _notificationService.SendAsync(new CreateNotificationDto
            {
                Title = "Low Stock Alert",
                Message = $"{result.Count} part(s) are below minimum stock: {partsText}.",
                RecipientUserId = null
            });
        }

        return result;
    }

    public async Task<PendingCreditsResultDto> CheckPendingCreditsAsync(int minDaysOutstanding = 30)
    {
        var cutoff = DateTime.UtcNow.AddDays(-minDaysOutstanding);

        // Finding unpaid credits overdue by more than one month.
        var pending = await _salesInvoiceRepo.FindAsync(i => !i.IsPaid && i.SaleDate < cutoff);

        var credits = new List<PendingCreditDto>();
        foreach (var invoice in pending.OrderBy(i => i.SaleDate))
        {
            var customer = await _userRepo.GetByIdAsync(invoice.CustomerId);

            var email = customer?.Email ?? string.Empty;
            var daysOutstanding = (int)Math.Floor((DateTime.UtcNow - invoice.SaleDate).TotalDays);

            credits.Add(new PendingCreditDto
            {
                InvoiceId = invoice.Id,
                CustomerId = invoice.CustomerId,
                CustomerEmail = email,
                SaleDate = invoice.SaleDate,
                AmountDue = invoice.FinalAmount,
                DaysOutstanding = daysOutstanding
            });
        }

        return new PendingCreditsResultDto
        {
            Count = credits.Count,
            Credits = credits
        };
    }

    public async Task<ReminderResultDto> SendRemindersAsync(int minDaysOutstanding = 30)
    {
        var pending = await CheckPendingCreditsAsync(minDaysOutstanding);

        var sentInvoiceIds = new List<int>();
        foreach (var credit in pending.Credits)
        {
            if (string.IsNullOrWhiteSpace(credit.CustomerEmail))
                continue;

            // Email reminder for customers with pending credits.
            await _emailService.SendCreditReminderEmailAsync(credit.CustomerEmail);

            // Storing a notification record as well.
            await _notificationService.SendAsync(new CreateNotificationDto
            {
                Title = "Payment Reminder",
                Message = $"You have an unpaid invoice (#{credit.InvoiceId}) pending for {credit.DaysOutstanding} days.",
                RecipientUserId = credit.CustomerId
            });

            sentInvoiceIds.Add(credit.InvoiceId);
        }

        return new ReminderResultDto
        {
            RemindersSent = sentInvoiceIds.Count,
            InvoiceIds = sentInvoiceIds
        };
    }
}