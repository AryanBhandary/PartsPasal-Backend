using System.ComponentModel.DataAnnotations;

namespace PartsPasal.Application.DTOs.Notifications;

public class CreateNotificationDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Optional user id. If it's null, it's sent to everyone.
    /// </summary>
    public int? RecipientUserId { get; set; }
}