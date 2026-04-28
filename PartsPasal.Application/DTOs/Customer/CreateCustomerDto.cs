namespace PartsPasal.Application.DTOs.Customer;

public class CreateCustomerDto
{
    public string Name { get; set; } = string.Empty;   
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }

    public string LicensePlate { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
}