using System.ComponentModel.DataAnnotations;
using PartsPasal.Domain.Enums;

namespace PartsPasal.Application.DTOs.Vendor;

public class UpdateVendorDto
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? ContactPerson { get; set; }

    [Required]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Address { get; set; }

    [Required]
    public VendorCategory Category { get; set; }
}