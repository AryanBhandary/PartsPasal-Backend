using PartsPasal.Application.DTOs.Customer;

namespace PartsPasal.Application.Interfaces;

public interface IStoreReviewService
{
    Task<int> SubmitReviewAsync(int userId, CreateStoreReviewDto dto);
    Task<string> GetAverageRatingAsync();
    Task<List<StoreReviewDto>> GetRecentReviewsAsync(int count);
}
