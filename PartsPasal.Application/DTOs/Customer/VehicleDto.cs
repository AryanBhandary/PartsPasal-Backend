namespace PartsPasal.Application.DTOs.Customer;

public class VehicleDto
{
    public int Id { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string? VIN { get; set; }
    public DateTime? LastServiceDate { get; set; }
    public int Mileage { get; set; }
}