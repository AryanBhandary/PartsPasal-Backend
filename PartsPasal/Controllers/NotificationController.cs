using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using PartsPasal.Application.DTOs.Notifications;
using PartsPasal.Application.Interfaces;

namespace PartsPasal.Controllers;

/// <summary>
/// Controller for retrieving and sending notifications.
/// </summary>
[Authorize(Roles = "Customer,Staff,Admin")]
[ApiController]
[Route("api/notifications")]
public class NotificationController(INotificationService notificationService) : ControllerBase
{
    private readonly INotificationService _notificationService = notificationService;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var notifications = await _notificationService.GetAllAsync();

        // Customers should only see their own notifications and broadcast messages.
        if (User.IsInRole("Customer"))
        {
            var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userIdText) || !int.TryParse(userIdText, out var userId))
                return Unauthorized("User ID not found in token.");

            notifications = notifications
                .Where(n => n.RecipientUserId == null || n.RecipientUserId == userId)
                .ToList();
        }

        return Ok(notifications);
    }

    [Authorize(Roles = "Staff,Admin")]
    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] CreateNotificationDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var id = await _notificationService.SendAsync(dto);

        return Ok(new
        {
            message = "Notification created successfully.",
            notificationId = id
        });
    }
}