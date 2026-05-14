using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PartsPasal.Domain.Enums;

namespace PartsPasal.Domain.Entities;

// Represents a customer request for an unavailable part
public class PartRequest
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    public string PartName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime RequestDate { get; set; } = DateTime.UtcNow;

    [Required]
    public PartRequestStatus Status { get; set; } = PartRequestStatus.Requested;
}
