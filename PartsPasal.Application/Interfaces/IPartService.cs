using PartsPasal.Application.DTOs.Inventory;

namespace PartsPasal.Application.Interfaces;

public interface IPartService
{
    Task<int> CreatePartAsync(CreatePartDto dto);

    Task<List<PartDto>> GetAllPartsAsync();

    Task<PartDto?> GetPartByIdAsync(int id);

    Task<bool> UpdatePartAsync(int id, UpdatePartDto dto);

    Task<bool> DeletePartAsync(int id);
}