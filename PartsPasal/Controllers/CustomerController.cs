using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartsPasal.Application.DTOs.Customer;
using PartsPasal.Application.Interfaces;

namespace PartsPasal.Controllers;

[Authorize(Roles = "Customer,Staff,Admin")]
[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpPost("book-appointment")]
    public async Task<IActionResult> BookAppointment(CreateAppointmentDto dto)
    {
        var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdText))
        {
            return Unauthorized("User ID not found in token.");
        }

        var userId = int.Parse(userIdText);

        var appointmentId = await _customerService.BookAppointmentAsync(userId, dto);

        return Ok(new
        {
            message = "Appointment booked successfully.",
            appointmentId
        });
    }

    [HttpGet("appointments")]
    public async Task<IActionResult> GetMyAppointments()
    {
        var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdText))
        {
            return Unauthorized("User ID not found in token.");
        }

        var userId = int.Parse(userIdText);

        var appointments = await _customerService.GetMyAppointmentsAsync(userId);

        return Ok(appointments);
    }

    [HttpPut("appointments/{id}")]
    public async Task<IActionResult> UpdateAppointment(int id, UpdateAppointmentDto dto)
    {
        var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdText))
        {
            return Unauthorized("User ID not found in token.");
        }

        var userId = int.Parse(userIdText);

        var updated = await _customerService.UpdateAppointmentAsync(userId, id, dto);

        if (!updated)
        {
            return NotFound("Appointment not found.");
        }

        return Ok(new
        {
            message = "Appointment updated successfully."
        });
    }

    [HttpDelete("appointments/{id}")]
    public async Task<IActionResult> CancelAppointment(int id)
    {
        var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdText))
        {
            return Unauthorized("User ID not found in token.");
        }

        var userId = int.Parse(userIdText);

        var deleted = await _customerService.CancelAppointmentAsync(userId, id);

        if (!deleted)
        {
            return NotFound("Appointment not found.");
        }

        return Ok(new
        {
            message = "Appointment cancelled successfully."
        });
    }

    [HttpPost("part-requests")]
    public async Task<IActionResult> CreatePartRequest(CreatePartRequestDto dto)
    {
        var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdText))
        {
            return Unauthorized("User ID not found in token.");
        }

        var userId = int.Parse(userIdText);

        var requestId = await _customerService.CreatePartRequestAsync(userId, dto);

        return Ok(new
        {
            message = "Part request submitted successfully.",
            requestId
        });
    }

    [HttpGet("part-requests")]
    public async Task<IActionResult> GetMyPartRequests()
    {
        var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdText))
        {
            return Unauthorized("User ID not found in token.");
        }

        var userId = int.Parse(userIdText);

        var requests = await _customerService.GetMyPartRequestsAsync(userId);

        return Ok(requests);
    }
}