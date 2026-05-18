using Microsoft.AspNetCore.Identity;
using PartsPasal.Application.DTOs.Staff;
using PartsPasal.Application.Interfaces;
using PartsPasal.Domain.Entities;
using PartsPasal.Domain.Enums;

namespace PartsPasal.Application.Services;

public class StaffManagementService(UserManager<User> userManager) : IStaffManagementService
{
    private readonly UserManager<User> _userManager = userManager;

    public async Task<(bool IsSuccess, string Message)> CreateStaffAsync(CreateStaffDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
        {
            return (false, "User with this email already exists.");
        }

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            UserName = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Address = dto.Address,
            RegistrationDate = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return (false, $"Staff creation failed: {errors}");
        }

        var roleResult = await _userManager.AddToRoleAsync(user, nameof(UserRole.Staff));
        if (!roleResult.Succeeded)
        {
            return (false, "Staff created but failed to assign Staff role.");
        }

        return (true, "Staff member created successfully.");
    }

    public async Task<IEnumerable<StaffDto>> GetAllStaffAsync()
    {
        var staffs = await _userManager.GetUsersInRoleAsync(nameof(UserRole.Staff));
        
        return staffs.Select(s => new StaffDto
        {
            Id = s.Id,
            Name = s.Name,
            Email = s.Email ?? string.Empty,
            PhoneNumber = s.PhoneNumber ?? string.Empty,
            Address = s.Address,
            RegistrationDate = s.RegistrationDate
        });
    }

    public async Task<StaffDto?> GetStaffByIdAsync(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return null;

        var isStaff = await _userManager.IsInRoleAsync(user, nameof(UserRole.Staff));
        if (!isStaff) return null;

        return new StaffDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            Address = user.Address,
            RegistrationDate = user.RegistrationDate
        };
    }

    public async Task<(bool IsSuccess, string Message)> UpdateStaffAsync(int id, UpdateStaffDto dto)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return (false, "Staff not found.");

        var isStaff = await _userManager.IsInRoleAsync(user, nameof(UserRole.Staff));
        if (!isStaff) return (false, "User is not a staff member.");

        user.Name = dto.Name;
        user.PhoneNumber = dto.PhoneNumber;
        user.Address = dto.Address;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return (false, $"Update failed: {errors}");
        }

        return (true, "Staff updated successfully.");
    }

    public async Task<(bool IsSuccess, string Message)> DeleteStaffAsync(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return (false, "Staff not found.");

        var isStaff = await _userManager.IsInRoleAsync(user, nameof(UserRole.Staff));
        if (!isStaff) return (false, "User is not a staff member.");

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return (false, $"Deletion failed: {errors}");
        }

        return (true, "Staff deleted successfully.");
    }

    public async Task<(bool IsSuccess, string Message)> AssignStaffRoleAsync(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return (false, "User not found.");

        var isStaff = await _userManager.IsInRoleAsync(user, nameof(UserRole.Staff));
        if (isStaff) return (false, "User already has the Staff role.");

        var result = await _userManager.AddToRoleAsync(user, nameof(UserRole.Staff));
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return (false, $"Role assignment failed: {errors}");
        }

        return (true, "Staff role assigned successfully.");
    }
}
