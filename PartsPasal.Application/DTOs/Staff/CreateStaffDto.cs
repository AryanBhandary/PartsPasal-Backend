using System.ComponentModel.DataAnnotations;

namespace PartsPasal.Application.DTOs.Staff;

public class CreateStaffDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;

    public string? Address { get; set; }

    [Required]
    public string SpecializedSkill { get; set; } = string.Empty;
}
