using System.Collections.Generic;
using PartsPasal.Application.DTOs.Sales;

namespace PartsPasal.Application.DTOs.Staff;

public class EndAppointmentDto
{
    public List<CreateSaleItemDto> Items { get; set; } = new();
    public bool IsPaid { get; set; } = true;
}
