namespace PartsPasal.Application.DTOs.Customer;

public class StoreReviewDto
{
    public int Id { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
    public string RatingStars { get; set; } = string.Empty; // e.g., "⭐⭐⭐⭐⭐"
    public DateTime ReviewDate { get; set; }
}
