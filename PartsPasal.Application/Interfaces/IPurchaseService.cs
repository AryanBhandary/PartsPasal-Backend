using PartsPasal.Application.DTOs.Purchase;

namespace PartsPasal.Application.Interfaces;

public interface IPurchaseService
{
    Task<int?> CreatePurchaseAsync(CreatePurchaseDto dto);

    Task<List<PurchaseDto>> GetAllPurchasesAsync();

    Task<PurchaseDto?> GetPurchaseByIdAsync(int id);
}