using Microsoft.AspNetCore.Mvc;
using PartsPasal.Application.Interfaces;

namespace PartsPasal.Controllers;

/// <summary>
/// General controller for part-related information accessible based on permissions.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PartsController : ControllerBase
{
    // [HttpGet("{id}")] - Get details of a specific part
    // [HttpGet("search")] - Search parts by category or name
}
