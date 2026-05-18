using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PartsPasal.Application.DTOs.Sales;
using PartsPasal.Application.Interfaces;

namespace PartsPasal.Controllers;

[Authorize(Roles = "Staff,Admin")]
[Route("api/sales")]
[ApiController]
public class SalesController : ControllerBase
{
    private readonly ISalesService _salesService;

    public SalesController(ISalesService salesService)
    {
        _salesService = salesService;
    }

    [HttpPost]
    public async Task<IActionResult> SellParts(CreateSaleDto dto)
    {
        var result = await _salesService.SellPartsAsync(dto);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _salesService.GetAllSalesAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _salesService.GetSaleByIdAsync(id);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPut("{id}/pay")]
    public async Task<IActionResult> MarkAsPaid(int id)
    {
        var success = await _salesService.MarkAsPaidAsync(id);
        if (!success)
            return BadRequest(new { message = "Invoice not found or already paid." });

        return Ok(new { message = "Invoice marked as paid successfully." });
    }
}