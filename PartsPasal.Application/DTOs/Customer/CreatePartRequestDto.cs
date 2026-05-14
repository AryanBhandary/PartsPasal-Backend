using System.ComponentModel.DataAnnotations;

namespace PartsPasal.Application.DTOs.Customer;

public class CreatePartRequestDto
{
    [Required]
    [MaxLength(250)]
    public string PartNameOrDescription { get; set; } = string.Empty;
}