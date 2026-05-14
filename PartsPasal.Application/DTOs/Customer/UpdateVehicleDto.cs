using System.ComponentModel.DataAnnotations;

namespace PartsPasal.Application.DTOs.Customer;

public class UpdateVehicleDto
{
    [Required(AllowEmptyStrings = false)]
    [MaxLength(20)]
    public string LicensePlate { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false)]
    [MaxLength(100)]
    public string Model { get; set; } = string.Empty;

    [Range(1900, 2100)]
    public int Year { get; set; }

    [Required(ErrorMessage = "VIN is required.")]
    [MaxLength(50)]
    public string VIN { get; set; } = string.Empty;

    public DateTime? LastServiceDate { get; set; }

    [Range(0, 2000000)]
    public int Mileage { get; set; }
}