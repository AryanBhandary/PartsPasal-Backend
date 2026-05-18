using PartsPasal.Application.DTOs.Customer;
using PartsPasal.Application.Interfaces;
using PartsPasal.Domain.Entities;

namespace PartsPasal.Application.Services;

public class StoreReviewService : IStoreReviewService
{
    private readonly IRepositoryBase<StoreReview> _storeReviewRepository;
    private readonly IRepositoryBase<User> _userRepository;

    public StoreReviewService(IRepositoryBase<StoreReview> storeReviewRepository, IRepositoryBase<User> userRepository)
    {
        _storeReviewRepository = storeReviewRepository;
        _userRepository = userRepository;
    }

    public async Task<int> SubmitReviewAsync(int userId, CreateStoreReviewDto dto)
    {
        // Check if the user has already submitted a store review to prevent spamming
        var existingReviews = await _storeReviewRepository.FindAsync(r => r.UserId == userId);
        var existingReview = existingReviews.FirstOrDefault();

        if (existingReview != null)
        {
            // Update existing review instead of creating a new one
            existingReview.Rating = dto.Rating;
            existingReview.ReviewDate = DateTime.UtcNow;
            
            _storeReviewRepository.Update(existingReview);
            await _storeReviewRepository.SaveChangesAsync();
            return existingReview.Id;
        }

        var review = new StoreReview
        {
            UserId = userId,
            Rating = dto.Rating,
            ReviewDate = DateTime.UtcNow
        };

        await _storeReviewRepository.AddAsync(review);
        await _storeReviewRepository.SaveChangesAsync();

        return review.Id;
    }

    public async Task<string> GetAverageRatingAsync()
    {
        var reviews = await _storeReviewRepository.GetAllAsync();
        
        if (!reviews.Any())
        {
            return "No ratings yet";
        }

        double average = reviews.Average(r => r.Rating);
        return $"{average:F1} ⭐";
    }

    public async Task<List<StoreReviewDto>> GetRecentReviewsAsync(int count)
    {
        var reviews = await _storeReviewRepository.GetAllAsync();
        
        var recentReviews = reviews
            .OrderByDescending(r => r.ReviewDate)
            .Take(count)
            .ToList();

        var result = new List<StoreReviewDto>();

        foreach (var review in recentReviews)
        {
            var user = await _userRepository.GetByIdAsync(review.UserId);
            string stars = new string('⭐', review.Rating);

            result.Add(new StoreReviewDto
            {
                Id = review.Id,
                ReviewerName = user?.Name ?? "Anonymous",
                RatingStars = stars,
                ReviewDate = review.ReviewDate
            });
        }

        return result;
    }
}
