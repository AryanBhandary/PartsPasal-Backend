using PartsPasal.Application.DTOs.System;

namespace PartsPasal.Application.Interfaces;

/// <summary>
/// These are internal automation methods.
/// </summary>
public interface ISystemAutomationService
{
    Task<LowStockCheckResultDto> CheckLowStockAsync();
    Task<PendingCreditsResultDto> CheckPendingCreditsAsync(int minDaysOutstanding = 30);
    Task<ReminderResultDto> SendRemindersAsync(int minDaysOutstanding = 30);
}