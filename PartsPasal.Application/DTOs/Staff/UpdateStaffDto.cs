using System.ComponentModel.DataAnnotations;

namespace PartsPasal.Application.DTOs.Staff;

public class UpdateStaffDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;

    public string? Address { get; set; }
}
