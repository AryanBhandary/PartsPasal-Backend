using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartsPasal.Application.DTOs.Customer;
using PartsPasal.Application.Interfaces;

namespace PartsPasal.Controllers;

/// <summary>
/// Controller for staff members handling customers and sales.
/// Features: Customer registration, Sales invoices, Search customers, Reports.
/// </summary>
[Authorize(Roles = "Staff,Admin")]
[ApiController]
[Route("api/[controller]")]
public class StaffController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public StaffController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpPost("register-customer")]
    public async Task<IActionResult> RegisterCustomer([FromBody] CreateCustomerDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var customerId = await _customerService.RegisterCustomerByStaffAsync(dto);
            return Ok(new { message = "Customer registered successfully.", customerId });
        }
        catch (System.InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("customers")]
    public async Task<IActionResult> GetCustomers([FromQuery] string? query = null)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }
        else
        {
            var customers = await _customerService.SearchCustomersAsync(query);
            return Ok(customers);
        }
    }

    [HttpPost("customers/{customerId}/vehicles")]
    public async Task<IActionResult> AddVehicleForCustomer(int customerId, [FromBody] CreateVehicleDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var customer = await _customerService.GetCustomerByIdAsync(customerId);
        if (customer == null)
        {
            return NotFound(new { message = "Customer not found." });
        }

        try
        {
            var vehicleId = await _customerService.AddVehicleAsync(customerId, dto);
            return Ok(new { message = "Vehicle added successfully.", vehicleId });
        }
        catch (System.Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("customers/{customerId}/history")]
    public async Task<IActionResult> GetCustomerHistory(int customerId)
    {
        var customer = await _customerService.GetCustomerByIdAsync(customerId);
        if (customer == null)
        {
            return NotFound(new { message = "Customer not found." });
        }

        var history = await _customerService.GetCustomerHistoryAsync(customerId);
        return Ok(history);
    }
}
