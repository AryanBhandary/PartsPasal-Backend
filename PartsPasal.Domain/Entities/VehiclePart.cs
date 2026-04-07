using System.ComponentModel.DataAnnotations;

namespace PartsPasal.Domain.Entities;

/// <summary>
/// Represents a vehicle part in the inventory.
/// Includes details for stock management, pricing, and category.
/// Required Feature: Admin can add, edit, delete parts; Stock alerts if < 10.
/// </summary>
public class VehiclePart
{
    // Id, Name, Description, Category, Price, StockQuantity, MinStockThreshold (default 10)
}
