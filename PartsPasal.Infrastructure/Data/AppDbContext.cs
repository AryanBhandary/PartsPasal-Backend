using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PartsPasal.Domain.Entities;
using PartsPasal.Domain.Enums;

namespace PartsPasal.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User, IdentityRole<int>, int>(options)
{
    public DbSet<VehiclePart> VehicleParts => Set<VehiclePart>();
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<PurchaseInvoice> PurchaseInvoices => Set<PurchaseInvoice>();
    public DbSet<PurchaseInvoiceItem> PurchaseInvoiceItems => Set<PurchaseInvoiceItem>();
    public DbSet<SalesInvoice> SalesInvoices => Set<SalesInvoice>();
    public DbSet<SalesInvoiceItem> SalesInvoiceItems => Set<SalesInvoiceItem>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<PartRequest> PartRequests => Set<PartRequest>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<StoreReview> StoreReviews => Set<StoreReview>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuring decimal precision for monetary columns
        modelBuilder.Entity<VehiclePart>().Property(p => p.Price).HasPrecision(18, 2);
        modelBuilder.Entity<SalesInvoice>().Property(s => s.TotalAmount).HasPrecision(18, 2);
        modelBuilder.Entity<SalesInvoice>().Property(s => s.DiscountAmount).HasPrecision(18, 2);
        modelBuilder.Entity<SalesInvoice>().Property(s => s.FinalAmount).HasPrecision(18, 2);
        modelBuilder.Entity<SalesInvoiceItem>().Property(si => si.SalePrice).HasPrecision(18, 2);
        modelBuilder.Entity<PurchaseInvoice>().Property(p => p.TotalAmount).HasPrecision(18, 2);
        modelBuilder.Entity<PurchaseInvoiceItem>().Property(pi => pi.UnitPrice).HasPrecision(18, 2);
        modelBuilder.Entity<User>().Property(u => u.TotalServiceSpent).HasPrecision(18, 2);

        // Configuring specific constraints (DeleteBehavior.Restrict to avoid multiple cascade paths)

        modelBuilder.Entity<SalesInvoice>()
            .HasOne(s => s.Customer).WithMany(u => u.Purchases)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SalesInvoice>()
            .HasOne(s => s.Staff).WithMany(u => u.SalesHandled)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.User).WithMany(u => u.Appointments)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Vehicle).WithMany(v => v.Appointments)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.User).WithMany(u => u.Reviews)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StoreReview>()
            .HasOne(sr => sr.User).WithMany(u => u.StoreReviews)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PartRequest>()
            .HasOne(p => p.User).WithMany(u => u.PartRequests)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SalesInvoiceItem>()
            .HasOne(si => si.VehiclePart).WithMany(vp => vp.SalesInvoiceItems)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PurchaseInvoice>()
            .HasOne(pi => pi.Vendor).WithMany(v => v.PurchaseInvoices)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PurchaseInvoiceItem>()
            .HasOne(pii => pii.VehiclePart).WithMany(vp => vp.PurchaseInvoiceItems)
            .OnDelete(DeleteBehavior.Restrict);

        // Seeding default roles
        modelBuilder.Entity<IdentityRole<int>>().HasData(
            new IdentityRole<int> { Id = 1, Name = nameof(UserRole.Customer), NormalizedName = "CUSTOMER", ConcurrencyStamp = "customer-role-stamp" },
            new IdentityRole<int> { Id = 2, Name = nameof(UserRole.Staff), NormalizedName = "STAFF", ConcurrencyStamp = "staff-role-stamp" },
            new IdentityRole<int> { Id = 3, Name = nameof(UserRole.Admin), NormalizedName = "ADMIN", ConcurrencyStamp = "admin-role-stamp" }
        );

        // Seeding default admin user
        var hasher = new PasswordHasher<User>();
        var adminUser = new User
        {
            Id = 1,
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@gmail.com",
            NormalizedEmail = "ADMIN@GMAIL.COM",
            EmailConfirmed = true,
            SecurityStamp = "admin-security-stamp",
            ConcurrencyStamp = "admin-concurrency-stamp",
            PasswordHash = "AQAAAAEAACcQAAAAEDMI+/0DRp5QbOuTKlmE6Somy+MxXdC0KtUJ+1FQl+DjbDq50oj3MD7WOT64GKpsbA==",
            RegistrationDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            TotalServiceSpent = 0,
            LockoutEnabled = false,
            AccessFailedCount = 0,
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
        };

        modelBuilder.Entity<User>().HasData(adminUser);

        // Assign admin role to admin user
        modelBuilder.Entity<IdentityUserRole<int>>().HasData(
            new IdentityUserRole<int> { UserId = 1, RoleId = 3 }
        );

        // Configure unique constraints for Vehicle (LicensePlate and VIN)
        modelBuilder.Entity<Vehicle>()
            .HasIndex(v => v.LicensePlate)
            .IsUnique();

        modelBuilder.Entity<Vehicle>()
            .HasIndex(v => v.VIN)
            .IsUnique();
    }
}
