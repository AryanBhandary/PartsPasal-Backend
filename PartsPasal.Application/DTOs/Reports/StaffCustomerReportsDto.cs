using System;
using System.Collections.Generic;

namespace PartsPasal.Application.DTOs.Reports;

public class RegularCustomerReportDto
{
    public int CustomerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public int AppointmentCount { get; set; }
    public decimal TotalServiceSpent { get; set; }
}

public class StaffCustomerReportsDto
{
    public List<CustomerReportDto> TopSpenders { get; set; } = new();
    public List<RegularCustomerReportDto> Regulars { get; set; } = new();
    public List<PendingCreditReportDto> PendingCredits { get; set; } = new();
}
