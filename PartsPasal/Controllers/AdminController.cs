using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartsPasal.Application.DTOs.Staff;
using PartsPasal.Application.Interfaces;
using PartsPasal.Infrastructure.Services;

namespace PartsPasal.Controllers;

/// <summary>
/// Controller for high-level administrative tasks.
/// Features: Staff management, Vendor details, Financial reports, Stock alerts.
/// </summary>
[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AdminController(
    IStaffManagementService staffManagementService,
    IReportingService reportingService,
    ICustomerService customerService) : ControllerBase
{
    private readonly IStaffManagementService _staffManagementService = staffManagementService;
    private readonly IReportingService _reportingService = reportingService;
    private readonly ICustomerService _customerService = customerService;

    [HttpPost("staffs")]
    public async Task<IActionResult> CreateStaff([FromBody] CreateStaffDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _staffManagementService.CreateStaffAsync(dto);
        if (!result.IsSuccess)
            return BadRequest(new { result.Message });

        return Ok(new { result.Message });
    }

    [HttpGet("staffs")]
    public async Task<IActionResult> GetAllStaff()
    {
        var staffs = await _staffManagementService.GetAllStaffAsync();
        return Ok(staffs);
    }

    [HttpGet("staffs/{id}")]
    public async Task<IActionResult> GetStaffById(int id)
    {
        var staff = await _staffManagementService.GetStaffByIdAsync(id);
        if (staff == null)
            return NotFound(new { Message = "Staff not found." });

        return Ok(staff);
    }

    [HttpPut("staffs/{id}")]
    public async Task<IActionResult> UpdateStaff(int id, [FromBody] UpdateStaffDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _staffManagementService.UpdateStaffAsync(id, dto);
        if (!result.IsSuccess)
            return BadRequest(new { result.Message });

        return Ok(new { result.Message });
    }

    [HttpDelete("staffs/{id}")]
    public async Task<IActionResult> DeleteStaff(int id)
    {
        var result = await _staffManagementService.DeleteStaffAsync(id);
        if (!result.IsSuccess)
            return BadRequest(new { result.Message });

        return Ok(new { result.Message });
    }

    [HttpPost("staffs/{id}/assign-role")]
    public async Task<IActionResult> AssignStaffRole(int id)
    {
        var result = await _staffManagementService.AssignStaffRoleAsync(id);
        if (!result.IsSuccess)
            return BadRequest(new { result.Message });

        return Ok(new { result.Message });
    }

    [HttpGet("reports/financial")]
    public async Task<IActionResult> GetFinancialReport(
        [FromQuery] string type = "daily",
        [FromQuery] string format = "json")
    {
        var report = await _reportingService.GetFinancialReportAsync(type);
        return ToReportResult(report, $"financial-{type}", format, ReportCsvExporter.ToCsvBytes(report));
    }

    [HttpGet("reports/customers/regulars")]
    public async Task<IActionResult> GetRegularCustomersReport([FromQuery] string format = "json")
    {
        var report = await _reportingService.GetRegularCustomersReportAsync();
        return ToReportResult(report, "customers-regulars", format, ReportCsvExporter.ToCsvBytes(report));
    }

    [HttpGet("reports/customers/high-spenders")]
    public async Task<IActionResult> GetHighSpendersReport(
        [FromQuery] int limit = 25,
        [FromQuery] string format = "json")
    {
        var report = await _reportingService.GetHighSpendersReportAsync(limit);
        return ToReportResult(report, "customers-high-spenders", format, ReportCsvExporter.ToCsvBytes(report));
    }

    [HttpGet("reports/customers/pending-credits")]
    public async Task<IActionResult> GetPendingCreditsReport([FromQuery] string format = "json")
    {
        var report = await _reportingService.GetPendingCreditsReportAsync();
        return ToReportResult(report, "customers-pending-credits", format, ReportCsvExporter.ToCsvBytes(report));
    }

    private IActionResult ToReportResult<T>(T report, string fileName, string format, byte[] csvBytes)
    {
        if (IsCsvFormat(format))
        {
            return File(csvBytes, "text/csv", $"{fileName}-{DateTime.UtcNow:yyyyMMdd}.csv");
        }

        return Ok(report);
    }

    private static bool IsCsvFormat(string format) =>
        format.Equals("csv", StringComparison.OrdinalIgnoreCase) ||
        format.Equals("download", StringComparison.OrdinalIgnoreCase);

    [HttpGet("part-requests")]
    public async Task<IActionResult> GetAllPartRequests()
    {
        var requests = await _customerService.GetAllPartRequestsAsync();
        return Ok(requests);
    }

    [HttpPut("part-requests/{id}/status")]
    public async Task<IActionResult> UpdatePartRequestStatus(int id, [FromBody] PartsPasal.Domain.Enums.PartRequestStatus status)
    {
        var success = await _customerService.UpdatePartRequestStatusAsync(id, status);
        if (!success)
            return NotFound(new { Message = "Part request not found." });

        return Ok(new { Message = "Part request status updated successfully." });
    }
}
