using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository;
/// <summary>
/// ReviewRepository handles all database operations for reviews.
/// It inherits from RepositoryBase<Review> which provides common CRUD operations like Create, Delete, Update.
/// 
/// WHY THIS PATTERN:
/// - By inheriting RepositoryBase, we avoid code duplication for basic operations
/// - We only implement custom logic specific to reviews (like calculating average rating)
/// - This follows the DRY principle (Don't Repeat Yourself)
/// </summary>
public class ReviewRepository : RepositoryBase<Review>, IReviewRepository
{
    public ReviewRepository(RepositoryContext repositoryContext) : base(repositoryContext) { }
    
    /// <summary>
    /// Gets all reviews for a specific product
    /// 
    /// WHY THIS METHOD:
    /// - We need to filter reviews by ProductId to show them on product pages
    /// - We include the User relationship so we can display the reviewer's name
    /// - trackChanges parameter allows EF Core optimization (use false for read-only queries)
    /// </summary>
    public async Task<IEnumerable<Review>> GetProductReviewsAsync(Guid productId, bool trackChanges) =>
        await FindByCondition(review => review.ProductId.Equals(productId), trackChanges)
            .Include(review => review.User) // Load user data to show reviewer name
            .OrderByDescending(review => review.CreatedAt) // Show newest reviews first
            .ToListAsync();
    
    /// <summary>
    /// Gets a single review by ID
    /// 
    /// WHY THIS METHOD:
    /// - Needed when user wants to delete or update their own review
    /// - Includes User data for permission checks (verify review owner matches current user)
    /// </summary>
    public async Task<Review?> GetReviewAsync(Guid reviewId, bool trackChanges) =>
        await FindByCondition(review => review.Id.Equals(reviewId), trackChanges)
            .Include(review => review.User)
            .SingleOrDefaultAsync(); // SingleOrDefault because ID is unique
    
    /// <summary>
    /// Creates a new review for a product
    /// 
    /// WHY THIS METHOD:
    /// - Sets the ProductId to ensure review is linked to correct product
    /// - Uses Create() from base class to add to DbSet
    /// - Must call SaveAsync() in service layer to persist to database
    /// </summary>
    public void CreateReview(Guid productId, Review review)
    {
        review.ProductId = productId; // Ensure review is linked to the product
        Create(review); // Inherited from RepositoryBase
    }
    
    /// <summary>
    /// Deletes a review
    /// 
    /// WHY THIS METHOD:
    /// - Simple wrapper around base Delete method
    /// - Kept for consistency and potential future custom logic
    /// </summary>
    public void DeleteReview(Review review) => Delete(review);
    
    /// <summary>
    /// Calculates the average rating for a product
    /// 
    /// WHY THIS METHOD:
    /// - Shows average star rating on product page (EC-007 requirement)
    /// - Uses LINQ Average() to calculate mean of all ratings
    /// - Returns 0 if no reviews exist yet
    /// - Query runs in database for performance (not in memory)
    /// </summary>
    public async Task<decimal> GetAverageRatingAsync(Guid productId)
    {
        // FirstOrDefaultAsync returns 0 if no reviews, otherwise returns average
        var averageRating = await FindByCondition(
                review => review.ProductId.Equals(productId), 
                trackChanges: false) // Read-only operation, no tracking needed
            .AverageAsync(review => review.Rating);
        
        return (decimal)Math.Round(averageRating, 2); // Round to 2 decimal places
    }

}