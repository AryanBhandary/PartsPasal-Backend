using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PartsPasal.Domain.Enums;

namespace PartsPasal.Domain.Entities;

// Represents a service appointment booked by a customer
public class Appointment
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    [Required]
    public int VehicleId { get; set; }

    [ForeignKey(nameof(VehicleId))]
    public Vehicle Vehicle { get; set; } = null!;

    [Required]
    public DateTime AppointmentDate { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

    // Navigation properties
    public Review? Review { get; set; }
}
