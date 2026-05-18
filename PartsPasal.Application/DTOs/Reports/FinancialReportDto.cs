namespace PartsPasal.Application.DTOs.Reports;

public class FinancialReportDto
{
    public string PeriodType { get; set; } = string.Empty;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal TotalSales { get; set; }
    public decimal TotalPurchases { get; set; }
    public decimal NetProfit { get; set; }
    public int SalesInvoiceCount { get; set; }
    public int PurchaseInvoiceCount { get; set; }
    public List<FinancialBreakdownDto> Breakdown { get; set; } = new();
}

public class FinancialBreakdownDto
{
    public string Label { get; set; } = string.Empty;
    public decimal SalesTotal { get; set; }
    public decimal PurchaseTotal { get; set; }
    public int SalesCount { get; set; }
    public int PurchaseCount { get; set; }
}
