using PartsPasal.Application.DTOs.Vendor;
using PartsPasal.Application.DTOs.Inventory;
using PartsPasal.Application.Interfaces;
using PartsPasal.Domain.Entities;

namespace PartsPasal.Application.Services;

public class VendorService : IVendorService
{
    private readonly IRepositoryBase<Vendor> _vendorRepository;
    private readonly IRepositoryBase<PurchaseInvoice> _purchaseRepository;
    private readonly IRepositoryBase<PurchaseInvoiceItem> _purchaseItemRepository;
    private readonly IRepositoryBase<VehiclePart> _partRepository;

    public VendorService(
        IRepositoryBase<Vendor> vendorRepository,
        IRepositoryBase<PurchaseInvoice> purchaseRepository,
        IRepositoryBase<PurchaseInvoiceItem> purchaseItemRepository,
        IRepositoryBase<VehiclePart> partRepository)
    {
        _vendorRepository = vendorRepository;
        _purchaseRepository = purchaseRepository;
        _purchaseItemRepository = purchaseItemRepository;
        _partRepository = partRepository;
    }

    public async Task<int> CreateVendorAsync(CreateVendorDto dto)
    {
        var vendor = new Vendor
        {
            Name = dto.Name,
            ContactPerson = dto.ContactPerson,
            Phone = dto.Phone,
            Email = dto.Email,
            Address = dto.Address,
            Category = dto.Category
        };

        await _vendorRepository.AddAsync(vendor);
        await _vendorRepository.SaveChangesAsync();

        return vendor.Id;
    }

    public async Task<List<VendorDto>> GetAllVendorsAsync()
    {
        var vendors = await _vendorRepository.GetAllAsync();

        return vendors.Select(v => new VendorDto
        {
            Id = v.Id,
            Name = v.Name,
            ContactPerson = v.ContactPerson,
            Phone = v.Phone,
            Email = v.Email,
            Address = v.Address,
            Category = v.Category.ToString()
        }).ToList();
    }

    public async Task<VendorDto?> GetVendorByIdAsync(int id)
    {
        var vendor = await _vendorRepository.GetByIdAsync(id);

        if (vendor == null) return null;

        return new VendorDto
        {
            Id = vendor.Id,
            Name = vendor.Name,
            ContactPerson = vendor.ContactPerson,
            Phone = vendor.Phone,
            Email = vendor.Email,
            Address = vendor.Address,
            Category = vendor.Category.ToString()
        };
    }

    public async Task<bool> UpdateVendorAsync(int id, UpdateVendorDto dto)
    {
        var vendor = await _vendorRepository.GetByIdAsync(id);

        if (vendor == null) return false;

        vendor.Name = dto.Name;
        vendor.ContactPerson = dto.ContactPerson;
        vendor.Phone = dto.Phone;
        vendor.Email = dto.Email;
        vendor.Address = dto.Address;
        vendor.Category = dto.Category;

        _vendorRepository.Update(vendor);
        await _vendorRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteVendorAsync(int id)
    {
        var vendor = await _vendorRepository.GetByIdAsync(id);

        if (vendor == null) return false;

        // OPTIONAL: prevent delete if vendor has purchases
        var purchases = await _purchaseRepository.FindAsync(p => p.VendorId == id);
        if (purchases.Any())
        {
            return false; // or throw business error
        }

        _vendorRepository.Delete(vendor);
        await _vendorRepository.SaveChangesAsync();

        return true;
    }

    public async Task<List<PartDto>> GetVendorsWithPartsAsync(int vendorId)
    {
        // Get all parts directly associated with this vendor
        var parts = await _partRepository.FindAsync(p => p.VendorId == vendorId);

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
}