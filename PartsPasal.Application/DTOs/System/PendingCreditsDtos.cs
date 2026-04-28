namespace PartsPasal.Application.DTOs.System;

public class PendingCreditDto
{
    public int InvoiceId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;

    public DateTime SaleDate { get; set; }
    public decimal AmountDue { get; set; }
    public int DaysOutstanding { get; set; }
}

public class PendingCreditsResultDto
{
    public int Count { get; set; }
    public List<PendingCreditDto> Credits { get; set; } = new();
}