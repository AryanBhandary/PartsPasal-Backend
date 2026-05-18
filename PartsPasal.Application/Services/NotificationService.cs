using PartsPasal.Application.DTOs.Notifications;
using PartsPasal.Application.Interfaces;
using PartsPasal.Domain.Entities;

namespace PartsPasal.Application.Services;

/// <summary>
/// Notification service for storing and sending notifications.
/// </summary>
public class NotificationService : INotificationService
{
    private readonly IRepositoryBase<Notification> _notificationRepo;

    public NotificationService(IRepositoryBase<Notification> notificationRepo)
    {
        _notificationRepo = notificationRepo;
    }

    public async Task<List<NotificationDto>> GetAllAsync()
    {
        var notifications = await _notificationRepo.GetAllAsync();

        return notifications
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                RecipientUserId = n.RecipientUserId,
                CreatedAt = n.CreatedAt
            })
            .ToList();
    }

    public async Task<int> SendAsync(CreateNotificationDto dto)
    {
        var notification = new Notification
        {
            Title = dto.Title,
            Message = dto.Message,
            RecipientUserId = dto.RecipientUserId,
            CreatedAt = DateTime.UtcNow
        };

        await _notificationRepo.AddAsync(notification);
        await _notificationRepo.SaveChangesAsync();

        return notification.Id;
    }
}