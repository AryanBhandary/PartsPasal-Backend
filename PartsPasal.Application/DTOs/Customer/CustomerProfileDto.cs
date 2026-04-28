namespace PartsPasal.Application.DTOs.Customer;

public class CustomerProfileDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }

    public DateTime RegistrationDate { get; set; }

    public decimal TotalServiceSpent { get; set; }

    public List<VehicleDto> Vehicles { get; set; } = new();
}