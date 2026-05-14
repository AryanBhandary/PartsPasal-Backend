using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartsPasal.Application.DTOs.Customer;
using PartsPasal.Application.Interfaces;

namespace PartsPasal.Controllers;

[Authorize(Roles = "Staff,Admin")]
[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto dto)
    {
        var id = await _customerService.RegisterCustomerByStaffAsync(dto);
        return Ok(new { customerId = id, message = "Customer created successfully" });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCustomers()
    {
        var customers = await _customerService.GetAllCustomersAsync();
        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustomerById(int id)
    {
        var customer = await _customerService.GetCustomerByIdAsync(id);
        if (customer == null) return NotFound("Customer not found.");
        return Ok(customer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCustomer(int id, [FromBody] UpdateCustomerProfileDto dto)
    {
        var updated = await _customerService.UpdateProfileAsync(id, dto);
        if (!updated) return NotFound("Customer not found.");
        return Ok(new { message = "Customer updated successfully" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var deleted = await _customerService.DeleteCustomerAsync(id);
        if (!deleted) return NotFound("Customer not found.");
        return Ok(new { message = "Customer deleted successfully" });
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchCustomers([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q)) return BadRequest("Query parameter 'q' is required.");
        var customers = await _customerService.SearchCustomersAsync(q);
        return Ok(customers);
    }
}
