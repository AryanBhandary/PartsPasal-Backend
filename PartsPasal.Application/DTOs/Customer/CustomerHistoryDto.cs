using System.Collections.Generic;

namespace PartsPasal.Application.DTOs.Customer;

public class CustomerHistoryDto
{
    public List<VehicleDto> Vehicles { get; set; } = new();
    public List<AppointmentDto> Appointments { get; set; } = new();
    public List<PartRequestDto> PartRequests { get; set; } = new();
    public List<SalesHistoryDto> Purchases { get; set; } = new();
}
