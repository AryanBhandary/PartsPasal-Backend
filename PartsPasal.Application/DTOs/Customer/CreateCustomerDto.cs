using System.ComponentModel.DataAnnotations;

namespace PartsPasal.Application.DTOs.Customer;

public class CreateCustomerDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;   

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Address { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}