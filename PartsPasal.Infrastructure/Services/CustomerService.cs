using PartsPasal.Application.DTOs.Customer;
using PartsPasal.Application.Interfaces;
using PartsPasal.Domain.Entities;

namespace PartsPasal.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly IRepositoryBase<Appointment> _appointmentRepository;
    private readonly IRepositoryBase<Vehicle> _vehicleRepository;
    private readonly IRepositoryBase<PartRequest> _partRequestRepository;


    public CustomerService(
        IRepositoryBase<Appointment> appointmentRepository,
        IRepositoryBase<Vehicle> vehicleRepository,
        IRepositoryBase<PartRequest> partRequestRepository)
    {
        _appointmentRepository = appointmentRepository;
        _vehicleRepository = vehicleRepository;
        _partRequestRepository = partRequestRepository;
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

    public async Task<bool> UpdateAppointmentAsync(int userId, int appointmentId, UpdateAppointmentDto dto)
    {
        var appointments = await _appointmentRepository.FindAsync(a =>
            a.Id == appointmentId && a.UserId == userId);

        var appointment = appointments.FirstOrDefault();

        if (appointment == null)
        {
            return false;
        }

        appointment.AppointmentDate = dto.AppointmentDate;
        appointment.Description = dto.Description;

        _appointmentRepository.Update(appointment);
        await _appointmentRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CancelAppointmentAsync(int userId, int appointmentId)
    {
        var appointments = await _appointmentRepository.FindAsync(a =>
            a.Id == appointmentId && a.UserId == userId);

        var appointment = appointments.FirstOrDefault();

        if (appointment == null)
        {
            return false;
        }

        _appointmentRepository.Delete(appointment);
        await _appointmentRepository.SaveChangesAsync();

        return true;
    }


    public async Task<int> CreatePartRequestAsync(int userId, CreatePartRequestDto dto)
    {
        var request = new PartRequest
        {
            UserId = userId,
            PartNameOrDescription = dto.PartNameOrDescription
        };

        await _partRequestRepository.AddAsync(request);
        await _partRequestRepository.SaveChangesAsync();

        return request.Id;
    }

    public async Task<List<PartRequestDto>> GetMyPartRequestsAsync(int userId)
    {
        var requests = await _partRequestRepository.FindAsync(r => r.UserId == userId);

        return requests.Select(r => new PartRequestDto
        {
            Id = r.Id,
            PartNameOrDescription = r.PartNameOrDescription,
            RequestDate = r.RequestDate,
            Status = r.Status.ToString()
        }).ToList();
    }
}