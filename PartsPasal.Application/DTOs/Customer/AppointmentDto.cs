namespace PartsPasal.Application.DTOs.Customer;

public class AppointmentDto
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
}