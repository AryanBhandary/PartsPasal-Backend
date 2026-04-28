using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using PartsPasal.Application.DTOs.Invoices;
using PartsPasal.Application.Interfaces;

namespace PartsPasal.Controllers;

/// <summary>
/// Controller for reading customer invoices and sending invoice emails.
/// </summary>
[Authorize(Roles = "Customer,Staff,Admin")]
[ApiController]
[Route("api/invoices")]
public class InvoiceController(IInvoiceService invoiceService) : ControllerBase
{
    private readonly IInvoiceService _invoiceService = invoiceService;

    /// <summary>
    /// Gets an invoice by id.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var invoice = await _invoiceService.GetInvoiceByIdAsync(id);

        if (invoice == null)
            return NotFound();

        // Customers can only access their own invoices.
        if (User.IsInRole("Customer"))
        {
            var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userIdText) || !int.TryParse(userIdText, out var userId))
                return Unauthorized("User ID not found in token.");

            if (invoice.CustomerId != userId)
                return Forbid();
        }

        return Ok(invoice);
    }

    /// <summary>
    /// Gets all invoices for a given customer.
    /// </summary>
    [HttpGet("customer/{customerId:int}")]
    public async Task<IActionResult> GetByCustomerId(int customerId)
    {
        // Customers can only access their own invoices.
        if (User.IsInRole("Customer"))
        {
            var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userIdText) || !int.TryParse(userIdText, out var userId))
                return Unauthorized("User ID not found in token.");

            if (customerId != userId)
                return Forbid();
        }

        var invoices = await _invoiceService.GetInvoicesByCustomerIdAsync(customerId);
        return Ok(invoices);
    }

    /// <summary>
    /// Sends the invoice email to the invoice's customer through their email.
    /// </summary>
    [HttpPost("send-email")]
    public async Task<IActionResult> SendEmail([FromBody] SendInvoiceEmailDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var sent = await _invoiceService.SendInvoiceEmailAsync(dto.InvoiceId);
        if (!sent)
            return NotFound(new { Message = "Invoice not found." });

        return Ok(new { Message = "Invoice email queued/sent successfully." });
    }
}