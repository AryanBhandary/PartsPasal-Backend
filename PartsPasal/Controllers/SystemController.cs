using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartsPasal.Application.Interfaces;

namespace PartsPasal.Controllers;

/// <summary>
/// Internal system automation endpoints.
/// </summary>
[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/system")]
public class SystemController(ISystemAutomationService systemAutomationService) : ControllerBase
{
    private readonly ISystemAutomationService _systemAutomationService = systemAutomationService;

    [HttpPost("check-low-stock")]
    public async Task<IActionResult> CheckLowStock()
    {
        var result = await _systemAutomationService.CheckLowStockAsync();
        return Ok(result);
    }

    [HttpPost("check-pending-credits")]
    public async Task<IActionResult> CheckPendingCredits()
    {
        var result = await _systemAutomationService.CheckPendingCreditsAsync();
        return Ok(result);
    }

    [HttpPost("send-reminders")]
    public async Task<IActionResult> SendReminders()
    {
        var result = await _systemAutomationService.SendRemindersAsync();
        return Ok(result);
    }
}