using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartsPasal.Application.DTOs.Customer;
using PartsPasal.Application.Interfaces;

namespace PartsPasal.Controllers;

public class StaffAddVehicleRequest : CreateVehicleDto
{
    public int CustomerId { get; set; }
}

[Authorize(Roles = "Staff,Admin")]
[ApiController]
[Route("api/vehicles")]
public class VehiclesController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public VehiclesController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpPost]
    public async Task<IActionResult> AddVehicle([FromBody] StaffAddVehicleRequest request)
    {
        var dto = new CreateVehicleDto
        {
            LicensePlate = request.LicensePlate,
            Model = request.Model,
            Year = request.Year,
            VIN = request.VIN,
            LastServiceDate = request.LastServiceDate,
            Mileage = request.Mileage
        };
        
        var vehicleId = await _customerService.AddVehicleAsync(request.CustomerId, dto);
        return Ok(new { vehicleId, message = "Vehicle added successfully" });
    }

    [HttpGet("customer/{customerId}")]
    public async Task<IActionResult> GetVehiclesByCustomer(int customerId)
    {
        var vehicles = await _customerService.GetMyVehiclesAsync(customerId);
        return Ok(vehicles);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVehicle(int id, [FromBody] UpdateVehicleDto dto)
    {
        var updated = await _customerService.UpdateVehicleByStaffAsync(id, dto);
        if (!updated) return NotFound("Vehicle not found.");
        return Ok(new { message = "Vehicle updated successfully" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVehicle(int id)
    {
        var deleted = await _customerService.DeleteVehicleByStaffAsync(id);
        if (!deleted) return NotFound("Vehicle not found.");
        return Ok(new { message = "Vehicle deleted successfully" });
    }
}
