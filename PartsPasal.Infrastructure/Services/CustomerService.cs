using PartsPasal.Application.DTOs.Customer;
using PartsPasal.Application.Interfaces;
using PartsPasal.Domain.Entities;

namespace PartsPasal.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly IRepositoryBase<Appointment> _appointmentRepository;
    private readonly IRepositoryBase<Vehicle> _vehicleRepository;

    public CustomerService(
        IRepositoryBase<Appointment> appointmentRepository,
        IRepositoryBase<Vehicle> vehicleRepository)
    {
        _appointmentRepository = appointmentRepository;
        _vehicleRepository = vehicleRepository;
    }

    public async Task<int> BookAppointmentAsync(int userId, CreateAppointmentDto dto)
    {
        var vehicles = await _vehicleRepository.FindAsync(v =>
            v.Id == dto.VehicleId && v.UserId == userId);

        if (!vehicles.Any())
        {
            throw new Exception("Vehicle not found or does not belong to this customer.");
        }

        var appointment = new Appointment
        {
            UserId = userId,
            VehicleId = dto.VehicleId,
            AppointmentDate = dto.AppointmentDate,
            Description = dto.Description
        };

        await _appointmentRepository.AddAsync(appointment);
        await _appointmentRepository.SaveChangesAsync();

        return appointment.Id;
    }

    public async Task<List<AppointmentDto>> GetMyAppointmentsAsync(int userId)
    {
        var appointments = await _appointmentRepository.FindAsync(a => a.UserId == userId);

        return appointments.Select(a => new AppointmentDto
        {
            Id = a.Id,
            VehicleId = a.VehicleId,
            AppointmentDate = a.AppointmentDate,
            Description = a.Description,
            Status = a.Status.ToString()
        }).ToList();
    }
}