using PartsPasal.Application.DTOs.Inventory;
using PartsPasal.Application.Interfaces;
using PartsPasal.Domain.Entities;

namespace PartsPasal.Application.Services;

public class PartService : IPartService
{
    private readonly IRepositoryBase<VehiclePart> _partRepository;

    public PartService(IRepositoryBase<VehiclePart> partRepository)
    {
        _partRepository = partRepository;
    }

    public async Task<int> CreatePartAsync(CreatePartDto dto)
    {
        var part = new VehiclePart
        {
            Name = dto.Name,
            Description = dto.Description,
            Category = dto.Category,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            MinStockThreshold = dto.MinStockThreshold
        };

        await _partRepository.AddAsync(part);
        await _partRepository.SaveChangesAsync();

        return part.Id;
    }

    public async Task<List<PartDto>> GetAllPartsAsync()
    {
        var parts = await _partRepository.GetAllAsync();

        return parts.Select(p => new PartDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Category = p.Category,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            MinStockThreshold = p.MinStockThreshold
        }).ToList();
    }

    public async Task<PartDto?> GetPartByIdAsync(int id)
    {
        var part = await _partRepository.GetByIdAsync(id);

        if (part == null) return null;

        return new PartDto
        {
            Id = part.Id,
            Name = part.Name,
            Description = part.Description,
            Category = part.Category,
            Price = part.Price,
            StockQuantity = part.StockQuantity,
            MinStockThreshold = part.MinStockThreshold
        };
    }

    public async Task<bool> UpdatePartAsync(int id, UpdatePartDto dto)
    {
        var part = await _partRepository.GetByIdAsync(id);

        if (part == null) return false;

        part.Name = dto.Name;
        part.Description = dto.Description;
        part.Category = dto.Category;
        part.Price = dto.Price;
        part.StockQuantity = dto.StockQuantity;
        part.MinStockThreshold = dto.MinStockThreshold;

        _partRepository.Update(part);
        await _partRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeletePartAsync(int id)
    {
        var part = await _partRepository.GetByIdAsync(id);

        if (part == null) return false;

        _partRepository.Delete(part);
        await _partRepository.SaveChangesAsync();

        return true;
    }
}