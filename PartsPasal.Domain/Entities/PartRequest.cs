using System.ComponentModel.DataAnnotations;

namespace PartsPasal.Domain.Entities;

/// <summary>
/// Represents a customer request for a part currently unavailable in stock.
/// Required Feature: Customers can request unavailable parts.
/// </summary>
public class PartRequest
{
    // Id, CustomerId, PartNameOrDescription, RequestDate, Status (Requested/Ordered/Arrived)
}
