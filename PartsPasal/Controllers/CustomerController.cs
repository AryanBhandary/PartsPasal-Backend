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

        if (appointmentId == null)
        {
            return BadRequest("Vehicle not found or does not belong to this customer.");
        }

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

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdText))
        {
            return Unauthorized("User ID not found in token.");
        }

        if (!int.TryParse(userIdText, out var userId))
        {
            return Unauthorized("Invalid user ID in token.");
        }

        var profile = await _customerService.GetProfileAsync(userId);

        if (profile == null)
        {
            return NotFound("Customer profile not found.");
        }

        return Ok(profile);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(UpdateCustomerProfileDto dto)
    {
        var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdText))
        {
            return Unauthorized("User ID not found in token.");
        }

        if (!int.TryParse(userIdText, out var userId))
        {
            return Unauthorized("Invalid user ID in token.");
        }

        var updated = await _customerService.UpdateProfileAsync(userId, dto);

        if (!updated)
        {
            return NotFound("Customer profile not found.");
        }

        return Ok(new
        {
            message = "Profile updated successfully."
        });
    }

    [HttpPost("vehicles")]
    public async Task<IActionResult> AddVehicle(CreateVehicleDto dto)
    {
        var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdText))
        {
            return Unauthorized("User ID not found in token.");
        }

        if (!int.TryParse(userIdText, out var userId))
        {
            return Unauthorized("Invalid user ID in token.");
        }

        try
        {
            var vehicleId = await _customerService.AddVehicleAsync(userId, dto);

            return Ok(new
            {
                message = "Vehicle added successfully.",
                vehicleId
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("vehicles")]
    public async Task<IActionResult> GetMyVehicles()
    {
        var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdText))
        {
            return Unauthorized("User ID not found in token.");
        }

        if (!int.TryParse(userIdText, out var userId))
        {
            return Unauthorized("Invalid user ID in token.");
        }

        var vehicles = await _customerService.GetMyVehiclesAsync(userId);

        return Ok(vehicles);
    }

    [HttpPut("vehicles/{id}")]
    public async Task<IActionResult> UpdateVehicle(int id, UpdateVehicleDto dto)
    {
        var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdText))
        {
            return Unauthorized("User ID not found in token.");
        }

        if (!int.TryParse(userIdText, out var userId))
        {
            return Unauthorized("Invalid user ID in token.");
        }

        try
        {
            var updated = await _customerService.UpdateVehicleAsync(userId, id, dto);

            if (!updated)
            {
                return NotFound(new { message = "Vehicle not found." });
            }

            return Ok(new
            {
                message = "Vehicle updated successfully."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("vehicles/{id}")]
    public async Task<IActionResult> DeleteVehicle(int id)
    {
        var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdText))
        {
            return Unauthorized("User ID not found in token.");
        }

        if (!int.TryParse(userIdText, out var userId))
        {
            return Unauthorized("Invalid user ID in token.");
        }

        var deleted = await _customerService.DeleteVehicleAsync(userId, id);

        if (!deleted)
        {
            return NotFound("Vehicle not found.");
        }

        return Ok(new
        {
            message = "Vehicle deleted successfully."
        });
    }

    
    [HttpGet("history")]
    public async Task<IActionResult> GetCustomerHistory()
    {
        var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdText))
        {
            return Unauthorized("User ID not found in token.");
        }

        if (!int.TryParse(userIdText, out var userId))
        {
            return Unauthorized("Invalid user ID in token.");
        }

        var history = await _customerService.GetCustomerHistoryAsync(userId);

        return Ok(history);
    }
}
