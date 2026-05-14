using System.ComponentModel.DataAnnotations;

namespace PartsPasal.Application.DTOs.Customer;

public class CreatePartRequestDto
{
    [Required(ErrorMessage = "Part name is required.")]
    [MaxLength(150)]
    public string PartName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}