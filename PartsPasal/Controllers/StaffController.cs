using Microsoft.AspNetCore.Mvc;
using PartsPasal.Application.Interfaces;

namespace PartsPasal.Controllers;

/// <summary>
/// Controller for staff members handling customers and sales.
/// Features: Customer registration, Sales invoices, Search customers, Reports.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class StaffController : ControllerBase
{
    // [HttpPost("register-customer")] - Register new customers with vehicle details
    // [HttpPost("sales-invoice")] - Handle part sales and create sales invoices (email attached)
    // [HttpGet("customers/{id}/history")] - View customer details, history, and vehicle info
    // [HttpGet("reports/customer")] - Generate customer-related reports (top spenders, pending credits)
    // [HttpGet("search")] - Search customers by vehicle number, phone, ID, or name
}
