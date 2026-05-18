using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartsPasal.Application.DTOs.Inventory;
using PartsPasal.Application.Interfaces;

namespace PartsPasal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PartsController : ControllerBase
{
    private readonly IPartService _partService;

    public PartsController(IPartService partService)
    {
        _partService = partService;
    }

    /// <summary>
    /// Public endpoint for customers to browse available (in-stock) parts.
    /// </summary>
    [HttpGet("available")]
    [Authorize(Roles = "Customer,Staff,Admin")]
    public async Task<IActionResult> GetAvailableParts()
    {
        var parts = await _partService.GetAllPartsAsync();
        var available = parts
            .Where(p => p.StockQuantity > 0 && p.Category != "Service")
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Description,
                p.Category,
                p.Price,
                p.StockQuantity
            })
            .ToList();
        return Ok(available);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreatePart(CreatePartDto dto)
    {
        var id = await _partService.CreatePartAsync(dto);

        return Ok(new
        {
            message = "Part created successfully",
            partId = id
        });
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllParts()
    {
        var parts = await _partService.GetAllPartsAsync();
        return Ok(parts);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPartById(int id)
    {
        var part = await _partService.GetPartByIdAsync(id);

        if (part == null)
            return NotFound("Part not found");

        return Ok(part);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePart(int id, UpdatePartDto dto)
    {
        var result = await _partService.UpdatePartAsync(id, dto);

        if (!result)
            return NotFound("Part not found");

        return Ok(new
        {
            message = "Part updated successfully"
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePart(int id)
    {
        var result = await _partService.DeletePartAsync(id);

        if (!result)
            return NotFound("Part not found");

        return Ok(new
        {
            message = "Part deleted successfully"
        });
    }
}