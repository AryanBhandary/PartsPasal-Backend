using Microsoft.AspNetCore.Mvc;

namespace PartsPasal.Controllers;

/// <summary>
/// Controller for authentication and basic registration for all user types.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    // [HttpPost("login")] - Authentication for Admin, Staff, and Customers
    // [HttpPost("customer/register")] - Self-registration for customers
}
