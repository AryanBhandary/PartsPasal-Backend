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

    [Required]
    [MaxLength(250)]
    public string PartNameOrDescription { get; set; } = string.Empty;

    public DateTime RequestDate { get; set; } = DateTime.UtcNow;

    [Required]
    public PartRequestStatus Status { get; set; } = PartRequestStatus.Requested;
}
