using PartsPasal.Application.DTOs.Vendor;

namespace PartsPasal.Application.Interfaces;

public interface IVendorService
{
    Task<int> CreateVendorAsync(CreateVendorDto dto);

    Task<List<VendorDto>> GetAllVendorsAsync();

    Task<VendorDto?> GetVendorByIdAsync(int id);

    Task<bool> UpdateVendorAsync(int id, UpdateVendorDto dto);

    Task<bool> DeleteVendorAsync(int id);

    Task<List<VendorDto>> GetVendorsWithPartsAsync(int vendorId);
}