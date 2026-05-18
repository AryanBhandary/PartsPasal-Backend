using PartsPasal.Application.DTOs.Vendor;
using PartsPasal.Application.DTOs.Inventory;

namespace PartsPasal.Application.Interfaces;

public interface IVendorService
{
    Task<int> CreateVendorAsync(CreateVendorDto dto);

    Task<List<VendorDto>> GetAllVendorsAsync();

    Task<VendorDto?> GetVendorByIdAsync(int id);

    Task<bool> UpdateVendorAsync(int id, UpdateVendorDto dto);

    Task<bool> DeleteVendorAsync(int id);

    Task<List<PartDto>> GetVendorsWithPartsAsync(int vendorId);
}