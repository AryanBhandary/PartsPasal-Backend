using Microsoft.AspNetCore.Mvc;
using PartsPasal.Application.Interfaces;

namespace PartsPasal.Controllers;

/// <summary>
/// Controller for customer self-service features.
/// Features: Booking, history, AI failed part alerts, part requests.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    // [HttpPost("book-appointment")] - Book service appointments
    // [HttpPost("request-part")] - Request unavailable parts
    // [HttpPost("submit-review")] - Submit service reviews
    // [HttpGet("history")] - View purchase and service history
    // [HttpGet("ai-prediction")] - Check AI alerts for potential part failures
}
