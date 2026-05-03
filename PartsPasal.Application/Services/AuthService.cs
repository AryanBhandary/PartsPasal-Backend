using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PartsPasal.Application.DTOs.Auth;
using PartsPasal.Application.Interfaces;
using PartsPasal.Domain.Entities;
using PartsPasal.Domain.Enums;

namespace PartsPasal.Application.Services;

public class AuthService(
    UserManager<User> userManager,
    IConfiguration configuration) : IAuthService
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly IConfiguration _configuration = configuration;

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
        {
            return new AuthResponseDto { IsSuccess = false, Message = "User with this email already exists." };
        }

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            UserName = dto.Email,
            PhoneNumber = dto.Phone,
            Address = dto.Address,
            RegistrationDate = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new AuthResponseDto { IsSuccess = false, Message = $"Registration failed: {errors}" };
        }

        // Assign default role
        await _userManager.AddToRoleAsync(user, nameof(UserRole.Customer));

        return new AuthResponseDto { IsSuccess = true, Message = "Registration successful." };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            return new AuthResponseDto { IsSuccess = false, Message = "Invalid email or password." };
        }

        var isValidPassword = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!isValidPassword)
        {
            return new AuthResponseDto { IsSuccess = false, Message = "Invalid email or password." };
        }

        var token = await GenerateJwtTokenAsync(user);

        return new AuthResponseDto
        {
            IsSuccess = true,
            Token = token,
            Message = "Login successful."
        };
    }

    private async Task<string> GenerateJwtTokenAsync(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, user.Name)
        };

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2), // Token valid for 2 hours
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
