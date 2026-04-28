using PartsPasal.Application.DTOs.Notifications;

namespace PartsPasal.Application.Interfaces;

/// <summary>
/// Service for creating and retrieving system notifications.
/// </summary>
public interface INotificationService
{
    Task<List<NotificationDto>> GetAllAsync();
    Task<int> SendAsync(CreateNotificationDto dto);
}