using System;

namespace PartsPasal.Application.DTOs.Staff;

public class StaffAppointmentDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public int VehicleId { get; set; }
    public string VehicleModel { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
}
