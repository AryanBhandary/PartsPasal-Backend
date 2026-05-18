using System.Globalization;
using System.Text;
using PartsPasal.Application.DTOs.Reports;

namespace PartsPasal.Infrastructure.Services;

public static class ReportCsvExporter
{
    public static byte[] ToCsvBytes(FinancialReportDto report)
    {
        var sb = new StringBuilder();
        AppendLine(sb, "Report Type", "Financial");
        AppendLine(sb, "Period Type", report.PeriodType);
        AppendLine(sb, "Period Start", FormatDate(report.PeriodStart));
        AppendLine(sb, "Period End", FormatDate(report.PeriodEnd));
        AppendLine(sb, "Total Sales", FormatMoney(report.TotalSales));
        AppendLine(sb, "Total Purchases", FormatMoney(report.TotalPurchases));
        AppendLine(sb, "Net Profit", FormatMoney(report.NetProfit));
        AppendLine(sb, "Sales Invoice Count", report.SalesInvoiceCount.ToString());
        AppendLine(sb, "Purchase Invoice Count", report.PurchaseInvoiceCount.ToString());
        sb.AppendLine();
        sb.AppendLine("Label,Sales Total,Purchase Total,Sales Count,Purchase Count");

        foreach (var row in report.Breakdown)
        {
            sb.AppendLine(string.Join(',',
                Escape(row.Label),
                FormatMoney(row.SalesTotal),
                FormatMoney(row.PurchaseTotal),
                row.SalesCount.ToString(),
                row.PurchaseCount.ToString()));
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    public static byte[] ToCsvBytes(CustomerListReportDto report)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Customer ID,Name,Email,Phone,Total Spent,Registration Date,Purchase Count");

        foreach (var customer in report.Customers)
        {
            sb.AppendLine(string.Join(',',
                customer.CustomerId.ToString(),
                Escape(customer.Name),
                Escape(customer.Email ?? string.Empty),
                Escape(customer.PhoneNumber ?? string.Empty),
                FormatMoney(customer.TotalServiceSpent),
                FormatDate(customer.RegistrationDate),
                customer.PurchaseCount.ToString()));
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    public static byte[] ToCsvBytes(PendingCreditsReportDto report)
    {
        var sb = new StringBuilder();
        AppendLine(sb, "Total Outstanding", FormatMoney(report.TotalOutstanding));
        AppendLine(sb, "Credit Count", report.Count.ToString());
        sb.AppendLine();
        sb.AppendLine("Invoice ID,Customer ID,Customer Name,Email,Sale Date,Amount Due,Days Outstanding");

        foreach (var credit in report.Credits)
        {
            sb.AppendLine(string.Join(',',
                credit.InvoiceId.ToString(),
                credit.CustomerId.ToString(),
                Escape(credit.CustomerName),
                Escape(credit.CustomerEmail ?? string.Empty),
                FormatDate(credit.SaleDate),
                FormatMoney(credit.AmountDue),
                credit.DaysOutstanding.ToString()));
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private static void AppendLine(StringBuilder sb, string key, string value) =>
        sb.AppendLine($"{Escape(key)},{Escape(value)}");

    private static string FormatMoney(decimal value) =>
        value.ToString("0.00", CultureInfo.InvariantCulture);

    private static string FormatDate(DateTime value) =>
        value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

    private static string Escape(string value)
    {
        if (value.Contains('"') || value.Contains(',') || value.Contains('\n'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }
}
