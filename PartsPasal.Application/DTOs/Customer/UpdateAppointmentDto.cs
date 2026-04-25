using System.ComponentModel.DataAnnotations;

namespace PartsPasal.Application.DTOs.Customer;

public class UpdateAppointmentDto
{
    [Required]
    public DateTime AppointmentDate { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}