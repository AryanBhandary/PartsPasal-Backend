using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartsPasal.Application.DTOs.Customer;
using PartsPasal.Application.DTOs.Staff;
using PartsPasal.Application.Interfaces;
using PartsPasal.Application.DTOs.Reports;

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

    [HttpGet("appointments")]
    public async Task<IActionResult> GetAppointments()
    {
        var appointments = await _customerService.GetAllAppointmentsForStaffAsync();
        return Ok(appointments);
    }

    [HttpPut("appointments/{id}/begin")]
    public async Task<IActionResult> BeginAppointment(int id)
    {
        var success = await _customerService.BeginAppointmentAsync(id);
        if (!success)
        {
            return BadRequest(new { message = "Appointment not found or not in Scheduled state." });
        }

        return Ok(new { message = "Appointment service started successfully." });
    }

    [HttpPost("appointments/{id}/end")]
    public async Task<IActionResult> EndAppointment(int id, [FromBody] EndAppointmentDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var staffIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(staffIdText))
        {
            return Unauthorized("Staff ID not found in token.");
        }
        var staffId = int.Parse(staffIdText);

        try
        {
            var invoice = await _customerService.EndAppointmentAsync(id, staffId, dto);
            if (invoice == null)
            {
                return BadRequest(new { message = "Appointment not found or not in progress." });
            }

            return Ok(new { message = "Appointment completed and invoice generated.", invoice });
        }
        catch (System.Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("customer-reports")]
    public async Task<IActionResult> GetCustomerReports()
    {
        var reports = await _customerService.GetCustomerReportsForStaffAsync();
        return Ok(reports);
    }
}
