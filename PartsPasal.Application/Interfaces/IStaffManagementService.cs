using PartsPasal.Application.DTOs.Staff;

namespace PartsPasal.Application.Interfaces;

public interface IStaffManagementService
{
    Task<(bool IsSuccess, string Message)> CreateStaffAsync(CreateStaffDto dto);
    Task<IEnumerable<StaffDto>> GetAllStaffAsync();
    Task<StaffDto?> GetStaffByIdAsync(int id);
    Task<(bool IsSuccess, string Message)> UpdateStaffAsync(int id, UpdateStaffDto dto);
    Task<(bool IsSuccess, string Message)> DeleteStaffAsync(int id);
    Task<(bool IsSuccess, string Message)> AssignStaffRoleAsync(int userId);
}
