using System.ComponentModel.DataAnnotations;

namespace PartsPasal.Application.DTOs.Customer;

public class UpdateAppointmentDto : IValidatableObject
{
    [Required]
    public DateTime AppointmentDate { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (AppointmentDate <= DateTime.UtcNow)
        {
            yield return new ValidationResult(
                "Appointment date must be in the future.",
                new[] { nameof(AppointmentDate) }
            );
        }
    }
}