using Entities.Models;

namespace Contracts;

public interface IReviewRepository
{
    Task<IEnumerable<Review>> GetProductReviewsAsync(Guid productId, bool trackChanges);
    Task<Review?> GetReviewAsync(Guid reviewId, bool trackChanges);
    void CreateReview(Guid productId, Review review);
    void DeleteReview(Review review);
    Task<decimal> GetAverageRatingAsync(Guid productId);
    Task<Review?> GetReviewByUserIdAsync(Guid productId, string userId, bool trackChanges);
}