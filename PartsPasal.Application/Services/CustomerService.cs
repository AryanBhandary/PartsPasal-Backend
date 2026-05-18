using Microsoft.AspNetCore.Identity;
using PartsPasal.Application.DTOs.Customer;
using PartsPasal.Application.DTOs.Staff;
using PartsPasal.Application.DTOs.Sales;
using PartsPasal.Application.Interfaces;
using PartsPasal.Domain.Entities;
using PartsPasal.Domain.Enums;

namespace PartsPasal.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly IRepositoryBase<Appointment> _appointmentRepository;
    private readonly IRepositoryBase<Vehicle> _vehicleRepository;
    private readonly IRepositoryBase<PartRequest> _partRequestRepository;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IRepositoryBase<SalesInvoice> _salesInvoiceRepository;
    private readonly UserManager<User> _userManager;
    private readonly IRepositoryBase<SalesInvoiceItem> _salesInvoiceItemRepository;
    private readonly IRepositoryBase<VehiclePart> _vehiclePartRepository;

    public CustomerService(
        IRepositoryBase<Appointment> appointmentRepository,
        IRepositoryBase<Vehicle> vehicleRepository,
        IRepositoryBase<PartRequest> partRequestRepository,
        IRepositoryBase<User> userRepository,
        IRepositoryBase<SalesInvoice> salesInvoiceRepository,
        UserManager<User> userManager,
        IRepositoryBase<SalesInvoiceItem> salesInvoiceItemRepository,
        IRepositoryBase<VehiclePart> vehiclePartRepository)
    {
        _appointmentRepository = appointmentRepository;
        _vehicleRepository = vehicleRepository;
        _partRequestRepository = partRequestRepository;
        _userRepository = userRepository;
        _salesInvoiceRepository = salesInvoiceRepository;
        _userManager = userManager;
        _salesInvoiceItemRepository = salesInvoiceItemRepository;
        _vehiclePartRepository = vehiclePartRepository;
    }

    // ================= EXISTING FEATURES =================

    public async Task<int?> BookAppointmentAsync(int userId, CreateAppointmentDto dto)
    {
        var vehicles = await _vehicleRepository.FindAsync(v =>
            v.Id == dto.VehicleId && v.UserId == userId);

        if (!vehicles.Any())
            return null;

        var appointment = new Appointment
        {
            UserId = userId,
            VehicleId = dto.VehicleId,
            // Npgsql requires DateTimeKind.Utc for 'timestamp with time zone' columns.
            // JSON deserialization produces DateTimeKind.Unspecified, so we normalize here.
            AppointmentDate = DateTime.SpecifyKind(dto.AppointmentDate, DateTimeKind.Utc),
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
            return false;

        appointment.AppointmentDate = DateTime.SpecifyKind(dto.AppointmentDate, DateTimeKind.Utc);
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
            return false;

        _appointmentRepository.Delete(appointment);
        await _appointmentRepository.SaveChangesAsync();

        return true;
    }

    public async Task<int> CreatePartRequestAsync(int userId, CreatePartRequestDto dto)
    {
        var request = new PartRequest
        {
            UserId = userId,
            PartName = dto.PartName.Trim(),
            Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
            RequestDate = DateTime.UtcNow,
            Status = PartRequestStatus.Requested
        };

        await _partRequestRepository.AddAsync(request);
        await _partRequestRepository.SaveChangesAsync();

        return request.Id;
    }

    public async Task<List<PartRequestDto>> GetMyPartRequestsAsync(int userId)
    {
        var requests = await _partRequestRepository.FindAsync(r => r.UserId == userId);

        return requests
            .OrderByDescending(r => r.RequestDate)
            .ThenByDescending(r => r.Id)
            .Select(r => new PartRequestDto
            {
                Id = r.Id,
                PartName = r.PartName,
                Description = r.Description,
                RequestDate = r.RequestDate,
                Status = r.Status.ToString()
            })
            .ToList();
    }

    public async Task<List<PartRequestDto>> GetAllPartRequestsAsync()
    {
        var requests = await _partRequestRepository.GetAllAsync();

        return requests
            .OrderByDescending(r => r.RequestDate)
            .ThenByDescending(r => r.Id)
            .Select(r => new PartRequestDto
            {
                Id = r.Id,
                PartName = r.PartName,
                Description = r.Description,
                RequestDate = r.RequestDate,
                Status = r.Status.ToString()
            })
            .ToList();
    }

    public async Task<bool> UpdatePartRequestStatusAsync(int requestId, PartRequestStatus status)
    {
        var request = await _partRequestRepository.GetByIdAsync(requestId);
        if (request == null) return false;

        request.Status = status;
        _partRequestRepository.Update(request);
        await _partRequestRepository.SaveChangesAsync();

        return true;
    }

    public async Task<CustomerProfileDto?> GetProfileAsync(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            return null;

        return new CustomerProfileDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,
            RegistrationDate = user.RegistrationDate,
            TotalServiceSpent = user.TotalServiceSpent
        };
    }

    public async Task<bool> UpdateProfileAsync(int userId, UpdateCustomerProfileDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            return false;

        user.Name = dto.Name;
        user.PhoneNumber = dto.PhoneNumber;
        user.Address = dto.Address;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
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
            LastServiceDate = dto.LastServiceDate.HasValue
                ? DateTime.SpecifyKind(dto.LastServiceDate.Value, DateTimeKind.Utc)
                : null,
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
            return false;

        vehicle.LicensePlate = dto.LicensePlate;
        vehicle.Model = dto.Model;
        vehicle.Year = dto.Year;
        vehicle.VIN = dto.VIN;
        vehicle.LastServiceDate = dto.LastServiceDate.HasValue
            ? DateTime.SpecifyKind(dto.LastServiceDate.Value, DateTimeKind.Utc)
            : null;
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
            return false;

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
        
        var purchaseIds = purchases.Select(p => p.Id).ToList();
        var allInvoiceItems = await _salesInvoiceItemRepository.FindAsync(item => purchaseIds.Contains(item.SalesInvoiceId));
        
        var partIds = allInvoiceItems.Select(item => item.VehiclePartId).Distinct().ToList();
        var allParts = await _vehiclePartRepository.FindAsync(part => partIds.Contains(part.Id));

        return new CustomerHistoryDto
        {
            Vehicles = vehicles,
            Appointments = appointments,
            PartRequests = partRequests,
            Purchases = purchases.Select(p => {
                var invoiceItems = allInvoiceItems.Where(item => item.SalesInvoiceId == p.Id).ToList();
                return new SalesHistoryDto
                {
                    Id = p.Id,
                    SaleDate = p.SaleDate,
                    TotalAmount = p.TotalAmount,
                    DiscountAmount = p.DiscountAmount,
                    FinalAmount = p.FinalAmount,
                    IsPaid = p.IsPaid,
                    Items = invoiceItems.Select(item => {
                        var part = allParts.FirstOrDefault(part => part.Id == item.VehiclePartId);
                        return new SalesHistoryItemDto
                        {
                            PartName = part?.Name ?? "Unknown Part",
                            Quantity = item.Quantity,
                            SalePrice = item.SalePrice
                        };
                    }).ToList()
                };
            }).ToList()
        };
    }


    // ================= STAFF FEATURES =================

    public async Task<int> RegisterCustomerByStaffAsync(CreateCustomerDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists.");
        }

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            UserName = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Address = dto.Address,
            RegistrationDate = DateTime.UtcNow,
            TotalServiceSpent = 0
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Registration failed: {errors}");
        }

        await _userManager.AddToRoleAsync(user, nameof(UserRole.Customer));

        return user.Id;
    }

    public async Task<List<CustomerProfileDto>> GetAllCustomersAsync()
    {
        var users = await _userManager.GetUsersInRoleAsync(nameof(UserRole.Customer));
        var result = new List<CustomerProfileDto>();

        foreach (var user in users)
        {
            var vehicles = await _vehicleRepository.FindAsync(v => v.UserId == user.Id);

            result.Add(new CustomerProfileDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                RegistrationDate = user.RegistrationDate,
                TotalServiceSpent = user.TotalServiceSpent,
                Vehicles = vehicles.Select(v => new VehicleDto
                {
                    Id = v.Id,
                    LicensePlate = v.LicensePlate,
                    Model = v.Model,
                    Year = v.Year,
                    VIN = v.VIN,
                    LastServiceDate = v.LastServiceDate,
                    Mileage = v.Mileage
                }).ToList()
            });
        }

        return result;
    }

    public async Task<CustomerProfileDto?> GetCustomerByIdAsync(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user == null)
            return null;

        var isCustomer = await _userManager.IsInRoleAsync(user, nameof(UserRole.Customer));
        if (!isCustomer)
            return null;

        var vehicles = await _vehicleRepository.FindAsync(v => v.UserId == id);

        return new CustomerProfileDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,
            RegistrationDate = user.RegistrationDate,
            TotalServiceSpent = user.TotalServiceSpent,
            Vehicles = vehicles.Select(v => new VehicleDto
            {
                Id = v.Id,
                LicensePlate = v.LicensePlate,
                Model = v.Model,
                Year = v.Year,
                VIN = v.VIN,
                LastServiceDate = v.LastServiceDate,
                Mileage = v.Mileage
            }).ToList()
        };
    }

    public async Task<List<CustomerProfileDto>> SearchCustomersAsync(string query)
    {
        var users = await _userManager.GetUsersInRoleAsync(nameof(UserRole.Customer));

        var matchedUsers = users.Where(u =>
            u.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            (u.PhoneNumber != null && u.PhoneNumber.Contains(query)) ||
            u.Id.ToString() == query
        ).ToList();

        var vehicles = await _vehicleRepository.FindAsync(v => v.LicensePlate.Contains(query));

        var vehicleUserIds = vehicles.Select(v => v.UserId).ToList();

        matchedUsers.AddRange(users.Where(u => vehicleUserIds.Contains(u.Id)));

        var distinctUsers = matchedUsers.GroupBy(u => u.Id).Select(g => g.First()).ToList();

        var result = new List<CustomerProfileDto>();

        foreach (var user in distinctUsers)
        {
            var customer = await GetCustomerByIdAsync(user.Id);
            if (customer != null)
                result.Add(customer);
        }

        return result;
    }

    private const decimal FixedServiceCharge = 4000.00m;

    public async Task<List<StaffAppointmentDto>> GetAllAppointmentsForStaffAsync()
    {
        var appointments = await _appointmentRepository.GetAllAsync();
        var result = new List<StaffAppointmentDto>();

        foreach (var a in appointments)
        {
            var user = await _userRepository.GetByIdAsync(a.UserId);
            var vehicle = await _vehicleRepository.GetByIdAsync(a.VehicleId);

            result.Add(new StaffAppointmentDto
            {
                Id = a.Id,
                CustomerId = a.UserId,
                CustomerName = user?.Name ?? "Unknown",
                CustomerEmail = user?.Email ?? "Unknown",
                VehicleId = a.VehicleId,
                VehicleModel = vehicle?.Model ?? "Unknown",
                LicensePlate = vehicle?.LicensePlate ?? "Unknown",
                AppointmentDate = a.AppointmentDate,
                Description = a.Description,
                Status = a.Status.ToString()
            });
        }

        return result.OrderByDescending(a => a.AppointmentDate).ToList();
    }

    public async Task<bool> BeginAppointmentAsync(int appointmentId)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
        if (appointment == null || appointment.Status != AppointmentStatus.Scheduled)
            return false;

        appointment.Status = AppointmentStatus.InProgress;
        _appointmentRepository.Update(appointment);
        await _appointmentRepository.SaveChangesAsync();
        return true;
    }

    public async Task<SalesInvoiceDto?> EndAppointmentAsync(int appointmentId, int staffId, EndAppointmentDto dto)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
        if (appointment == null || appointment.Status != AppointmentStatus.InProgress)
            return null;

        var customer = await _userRepository.GetByIdAsync(appointment.UserId);
        if (customer == null)
            throw new System.Exception("Customer not found");

        var invoice = new SalesInvoice
        {
            CustomerId = appointment.UserId,
            StaffId = staffId,
            SaleDate = DateTime.UtcNow,
            IsPaid = dto.IsPaid
        };

        await _salesInvoiceRepository.AddAsync(invoice);
        await _salesInvoiceRepository.SaveChangesAsync();

        decimal totalAmount = 0;

        foreach (var item in dto.Items)
        {
            var part = await _vehiclePartRepository.GetByIdAsync(item.PartId);
            if (part == null)
                throw new System.Exception("Part not found");

            if (part.Category != "Service")
            {
                if (part.StockQuantity < item.Quantity)
                    throw new System.Exception($"Not enough stock for {part.Name}");

                part.StockQuantity -= item.Quantity;
                _vehiclePartRepository.Update(part);
            }

            var lineTotal = part.Price * item.Quantity;
            totalAmount += lineTotal;

            var invoiceItem = new SalesInvoiceItem
            {
                SalesInvoiceId = invoice.Id,
                VehiclePartId = part.Id,
                Quantity = item.Quantity,
                SalePrice = part.Price
            };
            await _salesInvoiceItemRepository.AddAsync(invoiceItem);
        }

        // Add service charge statically
        totalAmount += FixedServiceCharge;

        await _vehiclePartRepository.SaveChangesAsync();
        await _salesInvoiceItemRepository.SaveChangesAsync();

        invoice.TotalAmount = totalAmount;

        // Apply loyalty discount (10% if total spent exceeds 5000)
        var isLoyalCustomer = customer.TotalServiceSpent > 5000m ||
                              (invoice.IsPaid && (customer.TotalServiceSpent + invoice.TotalAmount) > 5000m);

        invoice.DiscountAmount = isLoyalCustomer ? invoice.TotalAmount * 0.10m : 0m;
        invoice.FinalAmount = invoice.TotalAmount - invoice.DiscountAmount;

        _salesInvoiceRepository.Update(invoice);
        await _salesInvoiceRepository.SaveChangesAsync();

        // Update customer's lifetime spent amount only if paid (non-credit)
        if (invoice.IsPaid)
        {
            customer.TotalServiceSpent += invoice.FinalAmount;
            _userRepository.Update(customer);
            await _userRepository.SaveChangesAsync();
        }

        // Complete the appointment
        appointment.Status = AppointmentStatus.Completed;
        _appointmentRepository.Update(appointment);
        await _appointmentRepository.SaveChangesAsync();

        // Map and return the generated invoice
        var items = await _salesInvoiceItemRepository.FindAsync(i => i.SalesInvoiceId == invoice.Id);
        var invoiceDto = new SalesInvoiceDto
        {
            Id = invoice.Id,
            CustomerId = invoice.CustomerId,
            StaffId = invoice.StaffId,
            SaleDate = invoice.SaleDate,
            TotalAmount = invoice.TotalAmount,
            DiscountAmount = invoice.DiscountAmount,
            FinalAmount = invoice.FinalAmount,
            IsPaid = invoice.IsPaid
        };

        foreach (var item in items)
        {
            var part = await _vehiclePartRepository.GetByIdAsync(item.VehiclePartId);
            invoiceDto.Items.Add(new SalesInvoiceItemDto
            {
                PartId = item.VehiclePartId,
                PartName = part?.Name ?? string.Empty,
                Quantity = item.Quantity,
                SalePrice = item.SalePrice
            });
        }

        return invoiceDto;
    }
}