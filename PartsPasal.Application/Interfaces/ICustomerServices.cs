using PartsPasal.Application.DTOs.Customer;

namespace PartsPasal.Application.Interfaces;

public interface ICustomerService
{
    Task<int> BookAppointmentAsync(int userId, CreateAppointmentDto dto);
    Task<List<AppointmentDto>> GetMyAppointmentsAsync(int userId);


}