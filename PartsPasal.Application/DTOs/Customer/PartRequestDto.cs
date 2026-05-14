namespace PartsPasal.Application.DTOs.Customer;

public class PartRequestDto
{
    public int Id { get; set; }
    public string PartNameOrDescription { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public string Status { get; set; } = string.Empty;
}