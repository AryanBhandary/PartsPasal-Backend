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
}