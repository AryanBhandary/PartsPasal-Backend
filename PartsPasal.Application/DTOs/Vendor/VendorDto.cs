namespace PartsPasal.Application.DTOs.Vendor;

public class VendorDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? ContactPerson { get; set; }

    public string Phone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Address { get; set; }

    public string Category { get; set; } = string.Empty;
}