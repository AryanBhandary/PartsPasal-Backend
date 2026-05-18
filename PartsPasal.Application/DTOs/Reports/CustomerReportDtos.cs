namespace PartsPasal.Application.DTOs.Reports;

public class CustomerReportDto
{
    public int CustomerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public decimal TotalServiceSpent { get; set; }
    public DateTime RegistrationDate { get; set; }
    public int PurchaseCount { get; set; }
}

public class CustomerListReportDto
{
    public string ReportType { get; set; } = string.Empty;
    public int Count { get; set; }
    public List<CustomerReportDto> Customers { get; set; } = new();
}

public class PendingCreditReportDto
{
    public int InvoiceId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal AmountDue { get; set; }
    public int DaysOutstanding { get; set; }
}

public class PendingCreditsReportDto
{
    public int Count { get; set; }
    public decimal TotalOutstanding { get; set; }
    public List<PendingCreditReportDto> Credits { get; set; } = new();
}
