using System.ComponentModel.DataAnnotations;

namespace PartsPasal.Application.DTOs.Customer;

public class CreateStoreReviewDto
{
    [Required]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars.")]
    public int Rating { get; set; }
}
