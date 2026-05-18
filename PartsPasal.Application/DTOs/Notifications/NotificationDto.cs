namespace PartsPasal.Application.DTOs.Notifications;

public class NotificationDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// If its null, this notification is sent to everyone.
    /// </summary>
    public int? RecipientUserId { get; set; }

    public DateTime CreatedAt { get; set; }
}