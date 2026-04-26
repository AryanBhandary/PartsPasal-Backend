using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartsPasal.Application.DTOs.Purchase;
using PartsPasal.Application.Interfaces;

namespace PartsPasal.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class PurchaseController : ControllerBase
{
    private readonly IPurchaseService _purchaseService;

    public PurchaseController(IPurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePurchase(CreatePurchaseDto dto)
    {
        var result = await _purchaseService.CreatePurchaseAsync(dto);

        if (result == null)
            return BadRequest("Invalid vendor, part, or data");

        return Ok(new
        {
            message = "Purchase created and stock updated successfully",
            purchaseId = result
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPurchases()
    {
        var purchases = await _purchaseService.GetAllPurchasesAsync();
        return Ok(purchases);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPurchaseById(int id)
    {
        var purchase = await _purchaseService.GetPurchaseByIdAsync(id);

        if (purchase == null)
            return NotFound("Purchase not found");

        return Ok(purchase);
    }
}