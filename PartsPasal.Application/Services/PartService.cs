using PartsPasal.Application.DTOs.Inventory;
using PartsPasal.Application.Interfaces;
using PartsPasal.Domain.Entities;

namespace PartsPasal.Application.Services;

public class PartService : IPartService
{
    private readonly IRepositoryBase<VehiclePart> _partRepository;
    private readonly IRepositoryBase<Vendor> _vendorRepository;

    public PartService(
        IRepositoryBase<VehiclePart> partRepository,
        IRepositoryBase<Vendor> vendorRepository)
    {
        _partRepository = partRepository;
        _vendorRepository = vendorRepository;
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
            MinStockThreshold = dto.MinStockThreshold,
            VendorId = dto.VendorId
        };

        await _partRepository.AddAsync(part);
        await _partRepository.SaveChangesAsync();

        return part.Id;
    }

    public async Task<List<PartDto>> GetAllPartsAsync()
    {
        var parts = await _partRepository.GetAllAsync();
        var vendors = await _vendorRepository.GetAllAsync();
        var vendorDict = vendors.ToDictionary(v => v.Id, v => v.Name);

        return parts.Select(p => new PartDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Category = p.Category,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            MinStockThreshold = p.MinStockThreshold,
            VendorId = p.VendorId,
            VendorName = p.VendorId.HasValue && vendorDict.TryGetValue(p.VendorId.Value, out var vName) ? vName : null
        }).ToList();
    }

    public async Task<PartDto?> GetPartByIdAsync(int id)
    {
        var part = await _partRepository.GetByIdAsync(id);

        if (part == null) return null;

        string? vendorName = null;
        if (part.VendorId.HasValue)
        {
            var vendor = await _vendorRepository.GetByIdAsync(part.VendorId.Value);
            vendorName = vendor?.Name;
        }

        return new PartDto
        {
            Id = part.Id,
            Name = part.Name,
            Description = part.Description,
            Category = part.Category,
            Price = part.Price,
            StockQuantity = part.StockQuantity,
            MinStockThreshold = part.MinStockThreshold,
            VendorId = part.VendorId,
            VendorName = vendorName
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
        part.VendorId = dto.VendorId;

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