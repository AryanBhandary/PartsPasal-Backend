using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartsPasal.Domain.Entities;

public class StoreReview
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }

    public DateTime ReviewDate { get; set; } = DateTime.UtcNow;
}
