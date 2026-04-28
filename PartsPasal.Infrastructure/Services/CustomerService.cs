using PartsPasal.Application.DTOs.Customer;
using PartsPasal.Application.Interfaces;
using PartsPasal.Domain.Entities;

namespace PartsPasal.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly IRepositoryBase<Appointment> _appointmentRepository;
    private readonly IRepositoryBase<Vehicle> _vehicleRepository;
    private readonly IRepositoryBase<PartRequest> _partRequestRepository;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IRepositoryBase<SalesInvoice> _salesInvoiceRepository;


    public CustomerService(
        IRepositoryBase<Appointment> appointmentRepository,
        IRepositoryBase<Vehicle> vehicleRepository,
        IRepositoryBase<PartRequest> partRequestRepository,
        IRepositoryBase<User> userRepository,
        IRepositoryBase<SalesInvoice> salesInvoiceRepository)
    {
        _appointmentRepository = appointmentRepository;
        _vehicleRepository = vehicleRepository;
        _partRequestRepository = partRequestRepository;
        _userRepository = userRepository;
        _salesInvoiceRepository = salesInvoiceRepository;
    }

    public async Task<int?> BookAppointmentAsync(int userId, CreateAppointmentDto dto)
    {
        var vehicles = await _vehicleRepository.FindAsync(v =>
            v.Id == dto.VehicleId && v.UserId == userId);

        if (!vehicles.Any())
        {
            return null;
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

    public async Task<CustomerProfileDto?> GetProfileAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            return null;
        }

        return new CustomerProfileDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,
            RegistrationDate = user.RegistrationDate,
            TotalServiceSpent = user.TotalServiceSpent,
            IsLoyal = user.IsLoyal
        };
    }

    public async Task<bool> UpdateProfileAsync(int userId, UpdateCustomerProfileDto dto)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        user.Name = dto.Name;
        user.PhoneNumber = dto.PhoneNumber;
        user.Address = dto.Address;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        return true;
    }

    public async Task<int> AddVehicleAsync(int userId, CreateVehicleDto dto)
    {
        var vehicle = new Vehicle
        {
            UserId = userId,
            LicensePlate = dto.LicensePlate,
            Model = dto.Model,
            Year = dto.Year,
            VIN = dto.VIN,
            LastServiceDate = dto.LastServiceDate,
            Mileage = dto.Mileage
        };

        await _vehicleRepository.AddAsync(vehicle);
        await _vehicleRepository.SaveChangesAsync();

        return vehicle.Id;
    }

    public async Task<List<VehicleDto>> GetMyVehiclesAsync(int userId)
    {
        var vehicles = await _vehicleRepository.FindAsync(v => v.UserId == userId);

        return vehicles.Select(v => new VehicleDto
        {
            Id = v.Id,
            LicensePlate = v.LicensePlate,
            Model = v.Model,
            Year = v.Year,
            VIN = v.VIN,
            LastServiceDate = v.LastServiceDate,
            Mileage = v.Mileage
        }).ToList();
    }

    public async Task<bool> UpdateVehicleAsync(int userId, int vehicleId, UpdateVehicleDto dto)
    {
        var vehicles = await _vehicleRepository.FindAsync(v =>
            v.Id == vehicleId && v.UserId == userId);

        var vehicle = vehicles.FirstOrDefault();

        if (vehicle == null)
        {
            return false;
        }

        vehicle.LicensePlate = dto.LicensePlate;
        vehicle.Model = dto.Model;
        vehicle.Year = dto.Year;
        vehicle.VIN = dto.VIN;
        vehicle.LastServiceDate = dto.LastServiceDate;
        vehicle.Mileage = dto.Mileage;

        _vehicleRepository.Update(vehicle);
        await _vehicleRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteVehicleAsync(int userId, int vehicleId)
    {
        var vehicles = await _vehicleRepository.FindAsync(v =>
            v.Id == vehicleId && v.UserId == userId);

        var vehicle = vehicles.FirstOrDefault();

        if (vehicle == null)
        {
            return false;
        }

        _vehicleRepository.Delete(vehicle);
        await _vehicleRepository.SaveChangesAsync();

        return true;
    }

    public async Task<CustomerHistoryDto> GetCustomerHistoryAsync(int userId)
    {
        var vehicles = await GetMyVehiclesAsync(userId);
        var appointments = await GetMyAppointmentsAsync(userId);
        var partRequests = await GetMyPartRequestsAsync(userId);

        var purchases = await _salesInvoiceRepository.FindAsync(s => s.CustomerId == userId);

        return new CustomerHistoryDto
        {
            Vehicles = vehicles,
            Appointments = appointments,
            PartRequests = partRequests,
            Purchases = purchases.Select(p => new SalesHistoryDto
            {
                Id = p.Id,
                SaleDate = p.SaleDate,
                TotalAmount = p.TotalAmount,
                DiscountAmount = p.DiscountAmount,
                FinalAmount = p.FinalAmount,
                IsPaid = p.IsPaid
            }).ToList()
        };
    }


}