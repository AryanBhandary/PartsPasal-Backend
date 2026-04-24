using Microsoft.AspNetCore.Mvc;
using PartsPasal.Application.DTOs.Auth;
using PartsPasal.Application.Interfaces;

namespace PartsPasal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.RegisterAsync(dto);
        if (!result.IsSuccess)
        {
            return BadRequest(new { result.Message });
        }

        return Ok(new { result.Message });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.LoginAsync(dto);
        if (!result.IsSuccess)
        {
            return Unauthorized(new { result.Message });
        }

        return Ok(new { result.Token, result.Message });
    }

    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me([FromServices] Microsoft.AspNetCore.Identity.UserManager<PartsPasal.Domain.Entities.User> userManager)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return NotFound("User not found.");

        var roles = await userManager.GetRolesAsync(user);

        return Ok(new {
            user.Id,
            user.Email,
            user.UserName,
            user.Name,
            Roles = roles
        });
    }
}
