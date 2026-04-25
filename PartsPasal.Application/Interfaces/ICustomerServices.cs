using PartsPasal.Application.DTOs.Customer;

namespace PartsPasal.Application.Interfaces;

public interface ICustomerService
{
    // Book an appointment for a vehicle service
    Task<int> BookAppointmentAsync(int userId, CreateAppointmentDto dto);

    // Get a list of appointments for the logged-in user
    Task<List<AppointmentDto>> GetMyAppointmentsAsync(int userId);

    // Update an existing appointment 
    Task<bool> UpdateAppointmentAsync(int userId, int appointmentId, UpdateAppointmentDto dto);

    // Cancel an appointment
    Task<bool> CancelAppointmentAsync(int userId, int appointmentId);

    // Create a part request for a specific vehicle
    Task<int> CreatePartRequestAsync(int userId, CreatePartRequestDto dto);

    // Get a list of part requests for the user
    Task<List<PartRequestDto>> GetMyPartRequestsAsync(int userId);
}