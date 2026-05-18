using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace PartsPasal.Domain.Entities;

// Represents a user in the system (Admin, Staff, or Customer)
public class User : IdentityUser<int>
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    // Shared / Optional fields depending on role
    [MaxLength(300)]
    public string? Address { get; set; }

    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

    // Customer specific fields
    // Total amount the customer has spent on services/parts
    public decimal TotalServiceSpent { get; set; }

    // Staff specific fields
    [MaxLength(150)]
    public string? SpecializedSkill { get; set; }

    // Navigation properties (Customer Role)
    public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

    [InverseProperty(nameof(SalesInvoice.Customer))]
    public ICollection<SalesInvoice> Purchases { get; set; } = new List<SalesInvoice>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<PartRequest> PartRequests { get; set; } = new List<PartRequest>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    // Navigation properties (Staff Role)
    [InverseProperty(nameof(SalesInvoice.Staff))]
    public ICollection<SalesInvoice> SalesHandled { get; set; } = new List<SalesInvoice>();
}
