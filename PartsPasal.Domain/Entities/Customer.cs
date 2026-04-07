using System.ComponentModel.DataAnnotations;

namespace PartsPasal.Domain.Entities;

/// <summary>
/// Represents a customer in the system.
/// Required Feature: Staff registers customers; Customers can self-register.
/// Includes loyalty status and purchase history links.
/// </summary>
public class Customer
{
    // Id, Name, Email, Phone, Address, RegistrationDate, TotalServiceSpent, IsLoyal (spent > 5000)
    // Collection of Vehicles, SalesInvoices, Appointments
}
