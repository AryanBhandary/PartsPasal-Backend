using System.Collections.Generic;
using PartsPasal.Application.DTOs.Sales;

namespace PartsPasal.Application.DTOs.Staff;

public class EndAppointmentDto
{
    public List<CreateSaleItemDto> Items { get; set; } = new();
    public bool IsPaid { get; set; } = true;

    /// <summary>
    /// If true, the staff workflow will email the generated invoice to the customer.
    /// </summary>
    public bool SendInvoiceEmail { get; set; } = false;
}
