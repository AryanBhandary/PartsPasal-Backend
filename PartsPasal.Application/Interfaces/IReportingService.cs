using PartsPasal.Application.DTOs.Reports;

namespace PartsPasal.Application.Interfaces;

/// <summary>
/// Service for generating financial and customer reports.
/// </summary>
public interface IReportingService
{
    Task<FinancialReportDto> GetFinancialReportAsync(string periodType);
    Task<CustomerListReportDto> GetRegularCustomersReportAsync();
    Task<CustomerListReportDto> GetHighSpendersReportAsync(int limit = 25);
    Task<PendingCreditsReportDto> GetPendingCreditsReportAsync();
}
