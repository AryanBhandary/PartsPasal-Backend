namespace PartsPasal.Application.DTOs.System;

public class ReminderResultDto
{
    public int RemindersSent { get; set; }
    public List<int> InvoiceIds { get; set; } = new();
}