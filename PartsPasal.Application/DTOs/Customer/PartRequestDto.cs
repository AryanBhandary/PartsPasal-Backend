namespace PartsPasal.Application.DTOs.Customer;

public class PartRequestDto
{
    public int Id { get; set; }

    public string PartName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime RequestDate { get; set; }

    public string Status { get; set; } = string.Empty;
}