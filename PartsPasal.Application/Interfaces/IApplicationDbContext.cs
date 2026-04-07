using Microsoft.EntityFrameworkCore;
using PartsPasal.Domain.Entities;

namespace PartsPasal.Application.Interfaces;

/// <summary>
/// Database context interface for Clean Architecture.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<VehiclePart> VehicleParts { get; }
    DbSet<Vendor> Vendors { get; }
    DbSet<Customer> Customers { get; }
    DbSet<Staff> StaffMembers { get; }
    DbSet<Vehicle> Vehicles { get; }
    DbSet<PurchaseInvoice> PurchaseInvoices { get; }
    DbSet<SalesInvoice> SalesInvoices { get; }
    DbSet<Appointment> Appointments { get; }
    DbSet<PartRequest> PartRequests { get; }
    DbSet<Review> Reviews { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
