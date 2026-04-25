using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartsPasal.Application.DTOs.Staff;
using PartsPasal.Application.Interfaces;

namespace PartsPasal.Controllers;

/// <summary>
/// Controller for high-level administrative tasks.
/// Features: Staff management, Vendor details, Financial reports, Stock alerts.
/// </summary>
[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AdminController(IStaffManagementService staffManagementService) : ControllerBase
{
    private readonly IStaffManagementService _staffManagementService = staffManagementService;

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

    // [HttpGet("reports/financial")] - View financial reports (daily, monthly, yearly)
    // [HttpGet("inventory/status")] - View overall inventory and low stock alerts
    // [HttpPost("purchase-invoice")] - Create purchase invoices for stock updates
    // [HttpDelete("vendors/{id}")] - Manage vendor details (CRUD)
}
