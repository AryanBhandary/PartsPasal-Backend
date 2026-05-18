using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PartsPasal.Application.DTOs.Reports;
using PartsPasal.Application.Interfaces;
using PartsPasal.Domain.Entities;
using PartsPasal.Domain.Enums;
using PartsPasal.Infrastructure.Data;

namespace PartsPasal.Infrastructure.Services;

public class ReportingService(AppDbContext context, UserManager<User> userManager) : IReportingService
{
    private const decimal LoyaltyThreshold = 5000m;
    private readonly AppDbContext _context = context;
    private readonly UserManager<User> _userManager = userManager;

    public async Task<FinancialReportDto> GetFinancialReportAsync(string periodType)
    {
        var normalized = NormalizePeriodType(periodType);
        var (periodStart, periodEnd) = GetFinancialPeriodBounds(normalized);

        var allSales = await _context.SalesInvoices
            .AsNoTracking()
            .Where(s => s.SaleDate >= periodStart && s.SaleDate < periodEnd)
            .ToListAsync();

        var sales = allSales.Where(s => s.IsPaid).ToList();
        var creditSales = allSales.Where(s => !s.IsPaid).ToList();

        var purchases = await _context.PurchaseInvoices
            .AsNoTracking()
            .Where(p => p.Status == InvoiceStatus.Completed &&
                        p.PurchaseDate >= periodStart &&
                        p.PurchaseDate < periodEnd)
            .ToListAsync();

        var breakdown = normalized switch
        {
            "monthly" => BuildDailyBreakdown(sales, purchases, periodStart, periodEnd),
            "yearly" => BuildYearlyBreakdown(sales, purchases),
            _ => BuildHourlyBreakdown(sales, purchases, periodStart)
        };

        var totalSales = sales.Sum(s => s.FinalAmount);
        var totalPurchases = purchases.Sum(p => p.TotalAmount);
        var totalCredit = creditSales.Sum(s => s.FinalAmount);

        return new FinancialReportDto
        {
            PeriodType = normalized,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            TotalSales = totalSales,
            TotalPurchases = totalPurchases,
            TotalCredit = totalCredit,
            NetProfit = totalSales - totalPurchases,
            SalesInvoiceCount = sales.Count,
            PurchaseInvoiceCount = purchases.Count,
            Breakdown = breakdown
        };
    }

    public async Task<CustomerListReportDto> GetRegularCustomersReportAsync()
    {
        var customers = await GetCustomersAsync();
        var purchaseCounts = await GetPurchaseCountsByCustomerAsync();

        var regulars = customers
            .Where(c => c.TotalServiceSpent > LoyaltyThreshold)
            .OrderByDescending(c => c.TotalServiceSpent)
            .Select(c => MapCustomer(c, purchaseCounts.GetValueOrDefault(c.Id, 0)))
            .ToList();

        return new CustomerListReportDto
        {
            ReportType = "regulars",
            Count = regulars.Count,
            Customers = regulars
        };
    }

    public async Task<CustomerListReportDto> GetHighSpendersReportAsync(int limit = 25)
    {
        var customers = await GetCustomersAsync();
        var purchaseCounts = await GetPurchaseCountsByCustomerAsync();

        var highSpenders = customers
            .OrderByDescending(c => c.TotalServiceSpent)
            .Take(Math.Clamp(limit, 1, 100))
            .Select(c => MapCustomer(c, purchaseCounts.GetValueOrDefault(c.Id, 0)))
            .ToList();

        return new CustomerListReportDto
        {
            ReportType = "high-spenders",
            Count = highSpenders.Count,
            Customers = highSpenders
        };
    }

    public async Task<PendingCreditsReportDto> GetPendingCreditsReportAsync()
    {
        var pending = await _context.SalesInvoices
            .AsNoTracking()
            .Where(i => !i.IsPaid)
            .OrderBy(i => i.SaleDate)
            .ToListAsync();

        var customerIds = pending.Select(p => p.CustomerId).Distinct().ToList();
        var customers = await _context.Users
            .AsNoTracking()
            .Where(u => customerIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id);

        var credits = pending.Select(invoice =>
        {
            customers.TryGetValue(invoice.CustomerId, out var customer);
            return new PendingCreditReportDto
            {
                InvoiceId = invoice.Id,
                CustomerId = invoice.CustomerId,
                CustomerName = customer?.Name ?? "Unknown",
                CustomerEmail = customer?.Email,
                SaleDate = invoice.SaleDate,
                AmountDue = invoice.FinalAmount,
                DaysOutstanding = (int)Math.Floor((DateTime.UtcNow - invoice.SaleDate).TotalDays)
            };
        }).ToList();

        return new PendingCreditsReportDto
        {
            Count = credits.Count,
            TotalOutstanding = credits.Sum(c => c.AmountDue),
            Credits = credits
        };
    }

    private async Task<List<User>> GetCustomersAsync()
    {
        var usersInRole = await _userManager.GetUsersInRoleAsync(nameof(UserRole.Customer));
        return usersInRole.OrderByDescending(u => u.TotalServiceSpent).ToList();
    }

    private async Task<Dictionary<int, int>> GetPurchaseCountsByCustomerAsync()
    {
        return await _context.SalesInvoices
            .AsNoTracking()
            .GroupBy(s => s.CustomerId)
            .Select(g => new { CustomerId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.CustomerId, x => x.Count);
    }

    private static CustomerReportDto MapCustomer(User user, int purchaseCount) =>
        new()
        {
            CustomerId = user.Id,
            Name = user.Name,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            TotalServiceSpent = user.TotalServiceSpent,
            RegistrationDate = user.RegistrationDate,
            PurchaseCount = purchaseCount
        };

    private static string NormalizePeriodType(string periodType)
    {
        var value = periodType.Trim().ToLowerInvariant();
        return value is "monthly" or "yearly" ? value : "daily";
    }

    private static (DateTime Start, DateTime End) GetFinancialPeriodBounds(string periodType)
    {
        var now = DateTime.UtcNow;

        return periodType switch
        {
            "monthly" => (
                new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(1)),
            "yearly" => (DateTime.MinValue.ToUniversalTime(), now.Date.AddDays(1)),
            _ => (now.Date, now.Date.AddDays(1))
        };
    }

    private static List<FinancialBreakdownDto> BuildHourlyBreakdown(
        List<SalesInvoice> sales,
        List<PurchaseInvoice> purchases,
        DateTime dayStart)
    {
        var rows = new List<FinancialBreakdownDto>();

        for (var hour = 0; hour < 24; hour++)
        {
            var hourStart = dayStart.AddHours(hour);
            var hourEnd = hourStart.AddHours(1);

            var hourSales = sales.Where(s => s.SaleDate >= hourStart && s.SaleDate < hourEnd).ToList();
            var hourPurchases = purchases.Where(p => p.PurchaseDate >= hourStart && p.PurchaseDate < hourEnd).ToList();

            rows.Add(new FinancialBreakdownDto
            {
                Label = hourStart.ToString("HH:mm"),
                SalesTotal = hourSales.Sum(s => s.FinalAmount),
                PurchaseTotal = hourPurchases.Sum(p => p.TotalAmount),
                SalesCount = hourSales.Count,
                PurchaseCount = hourPurchases.Count
            });
        }

        return rows;
    }

    private static List<FinancialBreakdownDto> BuildDailyBreakdown(
        List<SalesInvoice> sales,
        List<PurchaseInvoice> purchases,
        DateTime periodStart,
        DateTime periodEnd)
    {
        var rows = new List<FinancialBreakdownDto>();

        for (var day = periodStart.Date; day < periodEnd.Date; day = day.AddDays(1))
        {
            var nextDay = day.AddDays(1);
            var daySales = sales.Where(s => s.SaleDate >= day && s.SaleDate < nextDay).ToList();
            var dayPurchases = purchases.Where(p => p.PurchaseDate >= day && p.PurchaseDate < nextDay).ToList();

            rows.Add(new FinancialBreakdownDto
            {
                Label = day.ToString("yyyy-MM-dd"),
                SalesTotal = daySales.Sum(s => s.FinalAmount),
                PurchaseTotal = dayPurchases.Sum(p => p.TotalAmount),
                SalesCount = daySales.Count,
                PurchaseCount = dayPurchases.Count
            });
        }

        return rows;
    }

    private static List<FinancialBreakdownDto> BuildMonthlyBreakdown(
        List<SalesInvoice> sales,
        List<PurchaseInvoice> purchases,
        DateTime periodStart,
        DateTime periodEnd)
    {
        var rows = new List<FinancialBreakdownDto>();

        for (var month = new DateTime(periodStart.Year, periodStart.Month, 1, 0, 0, 0, DateTimeKind.Utc);
             month < periodEnd;
             month = month.AddMonths(1))
        {
            var nextMonth = month.AddMonths(1);
            var monthSales = sales.Where(s => s.SaleDate >= month && s.SaleDate < nextMonth).ToList();
            var monthPurchases = purchases.Where(p => p.PurchaseDate >= month && p.PurchaseDate < nextMonth).ToList();

            rows.Add(new FinancialBreakdownDto
            {
                Label = month.ToString("yyyy-MM"),
                SalesTotal = monthSales.Sum(s => s.FinalAmount),
                PurchaseTotal = monthPurchases.Sum(p => p.TotalAmount),
                SalesCount = monthSales.Count,
                PurchaseCount = monthPurchases.Count
            });
        }

        return rows;
    }

    private static List<FinancialBreakdownDto> BuildYearlyBreakdown(
        List<SalesInvoice> sales,
        List<PurchaseInvoice> purchases)
    {
        var salesYears = sales.Select(s => s.SaleDate.Year);
        var purchaseYears = purchases.Select(p => p.PurchaseDate.Year);
        var years = salesYears.Concat(purchaseYears).Distinct().OrderBy(y => y).ToList();

        if (years.Count == 0)
            years.Add(DateTime.UtcNow.Year);

        return years.Select(year =>
        {
            var yearSales = sales.Where(s => s.SaleDate.Year == year).ToList();
            var yearPurchases = purchases.Where(p => p.PurchaseDate.Year == year).ToList();

            return new FinancialBreakdownDto
            {
                Label = year.ToString(),
                SalesTotal = yearSales.Sum(s => s.FinalAmount),
                PurchaseTotal = yearPurchases.Sum(p => p.TotalAmount),
                SalesCount = yearSales.Count,
                PurchaseCount = yearPurchases.Count
            };
        }).ToList();
    }
}
