using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PartsPasal.Application.Interfaces;

namespace PartsPasal.Infrastructure.Jobs;

/// <summary>
/// Periodically runs internal automation tasks:
/// - Low stock notifications for Admin (<10)
/// - Credit reminder emails for unpaid invoices older than 1 month
/// </summary>
public sealed class SystemAutomationHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SystemAutomationHostedService> _logger;

    private readonly TimeSpan _tickInterval;
    private readonly TimeSpan _lowStockInterval;
    private readonly TimeSpan _creditReminderInterval;
    private readonly int _minDaysOutstanding;

    public SystemAutomationHostedService(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<SystemAutomationHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        _tickInterval = TimeSpan.FromMinutes(configuration.GetValue("Automation:TickMinutes", 5));
        _lowStockInterval = TimeSpan.FromMinutes(configuration.GetValue("Automation:LowStockIntervalMinutes", 60));
        _creditReminderInterval = TimeSpan.FromMinutes(configuration.GetValue("Automation:CreditReminderIntervalMinutes", 1440));
        _minDaysOutstanding = configuration.GetValue("Automation:MinDaysOutstanding", 30);

        if (_tickInterval <= TimeSpan.Zero) _tickInterval = TimeSpan.FromMinutes(5);
        if (_lowStockInterval <= TimeSpan.Zero) _lowStockInterval = TimeSpan.FromMinutes(60);
        if (_creditReminderInterval <= TimeSpan.Zero) _creditReminderInterval = TimeSpan.FromMinutes(1440);
        if (_minDaysOutstanding <= 0) _minDaysOutstanding = 30;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "SystemAutomationHostedService started. LowStockInterval={LowStockInterval}. CreditReminderInterval={CreditReminderInterval}. MinDaysOutstanding={MinDaysOutstanding}.",
            _lowStockInterval, _creditReminderInterval, _minDaysOutstanding);

        var lastLowStockRun = DateTimeOffset.MinValue;
        var lastReminderRun = DateTimeOffset.MinValue;

        await RunLowStockAsync(stoppingToken);
        lastLowStockRun = DateTimeOffset.UtcNow;

        await RunCreditRemindersAsync(stoppingToken);
        lastReminderRun = DateTimeOffset.UtcNow;

        using var timer = new PeriodicTimer(_tickInterval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            var now = DateTimeOffset.UtcNow;

            if (now - lastLowStockRun >= _lowStockInterval)
            {
                await RunLowStockAsync(stoppingToken);
                lastLowStockRun = now;
            }

            if (now - lastReminderRun >= _creditReminderInterval)
            {
                await RunCreditRemindersAsync(stoppingToken);
                lastReminderRun = now;
            }
        }
    }

    private async Task RunLowStockAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var automation = scope.ServiceProvider.GetRequiredService<ISystemAutomationService>();
            var result = await automation.CheckLowStockAsync();

            _logger.LogInformation("Low stock check complete. Count={Count}.", result.Count);
        }
        catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Low stock check failed.");
        }
    }

    private async Task RunCreditRemindersAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var automation = scope.ServiceProvider.GetRequiredService<ISystemAutomationService>();
            var result = await automation.SendRemindersAsync(_minDaysOutstanding);

            _logger.LogInformation("Credit reminders complete. RemindersSent={Count}.", result.RemindersSent);
        }
        catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Credit reminder run failed.");
        }
    }
}