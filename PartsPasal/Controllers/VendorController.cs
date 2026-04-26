using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartsPasal.Application.DTOs.Vendor;
using PartsPasal.Application.Interfaces;

namespace PartsPasal.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class VendorsController : ControllerBase
{
    private readonly IVendorService _vendorService;

    public VendorsController(IVendorService vendorService)
    {
        _vendorService = vendorService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateVendor(CreateVendorDto dto)
    {
        var id = await _vendorService.CreateVendorAsync(dto);

        return Ok(new
        {
            message = "Vendor created successfully",
            vendorId = id
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllVendors()
    {
        var vendors = await _vendorService.GetAllVendorsAsync();
        return Ok(vendors);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVendorById(int id)
    {
        var vendor = await _vendorService.GetVendorByIdAsync(id);

        if (vendor == null)
            return NotFound("Vendor not found");

        return Ok(vendor);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVendor(int id, UpdateVendorDto dto)
    {
        var result = await _vendorService.UpdateVendorAsync(id, dto);

        if (!result)
            return NotFound("Vendor not found");

        return Ok(new
        {
            message = "Vendor updated successfully"
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVendor(int id)
    {
        var result = await _vendorService.DeleteVendorAsync(id);

        if (!result)
            return BadRequest("Vendor cannot be deleted or not found");

        return Ok(new
        {
            message = "Vendor deleted successfully"
        });
    }

    [HttpGet("{id}/parts")]
    public async Task<IActionResult> GetVendorParts(int id)
    {
        var parts = await _vendorService.GetVendorsWithPartsAsync(id);

        return Ok(parts);
    }
}