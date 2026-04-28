using PartsPasal.Application.DTOs.Sales;

namespace PartsPasal.Application.Interfaces;

public interface ISalesService
{
    Task<SalesInvoiceDto> SellPartsAsync(CreateSaleDto dto);
    Task<List<SalesInvoiceDto>> GetAllSalesAsync();
    Task<SalesInvoiceDto?> GetSaleByIdAsync(int id);
}