using Shared.DataTransferObjects;

namespace Service.Contracts;

public interface IReviewService
{
    Task<IEnumerable<ReviewDto>> GetProductReviewsAsync(Guid productId);
    Task<ReviewDto> CreateReviewAsync(Guid productId, ReviewForCreationDto reviewDto, string userId);
    Task<decimal> GetAverageRatingAsync(Guid productId);
    Task DeleteReviewAsync(Guid reviewId, string userId);
}