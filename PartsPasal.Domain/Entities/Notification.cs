using System.ComponentModel.DataAnnotations;

namespace PartsPasal.Domain.Entities;

/// <summary>
/// A notification record (which is stored and retrieved via API).
/// </summary>
public class Notification
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Nullable user id. If it's null, this notification sent to everyone.
    /// </summary>
    public int? RecipientUserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
