using PartsPasal.Application.DTOs.Customer;

namespace PartsPasal.Application.Interfaces;

public interface ICustomerService
{
    // Book an appointment for a vehicle service
    Task<int?> BookAppointmentAsync(int userId, CreateAppointmentDto dto);
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

    // Admin: Get all part requests
    Task<List<PartRequestDto>> GetAllPartRequestsAsync();

    // Admin: Update part request status
    Task<bool> UpdatePartRequestStatusAsync(int requestId, PartsPasal.Domain.Enums.PartRequestStatus status);

    // Get the profile information of the user
    Task<CustomerProfileDto?> GetProfileAsync(int userId);

    // Update the profile information of the user
    Task<bool> UpdateProfileAsync(int userId, UpdateCustomerProfileDto dto);

    // Add a vehicle 
    Task<int> AddVehicleAsync(int userId, CreateVehicleDto dto);

    // Get a list of vehicles 
    Task<List<VehicleDto>> GetMyVehiclesAsync(int userId);

    // Update a vehicle's information
    Task<bool> UpdateVehicleAsync(int userId, int vehicleId, UpdateVehicleDto dto);

    // Delete a vehicle
    Task<bool> DeleteVehicleAsync(int userId, int vehicleId);


    // Staff: Update vehicle
    Task<bool> UpdateVehicleByStaffAsync(int vehicleId, UpdateVehicleDto dto);

    // Staff: Delete vehicle
    Task<bool> DeleteVehicleByStaffAsync(int vehicleId);



    // Get customer history
    Task<CustomerHistoryDto> GetCustomerHistoryAsync(int userId);



    // Staff: Register new customer with vehicle
    Task<int> RegisterCustomerByStaffAsync(CreateCustomerDto dto);

    // Staff: View all customers with vehicles
    Task<List<CustomerProfileDto>> GetAllCustomersAsync();

    // Staff: View specific customer details
    Task<CustomerProfileDto?> GetCustomerByIdAsync(int id);

    // Staff: Search customers (name, phone, id, vehicle number)
    Task<List<CustomerProfileDto>> SearchCustomersAsync(string query);


    // Staff: Delete customer
    Task<bool> DeleteCustomerAsync(int id);

}