using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

/// <summary>
/// ReviewService handles all business logic for product reviews.
/// 
/// KEY RESPONSIBILITIES:
/// - Retrieve reviews for a product
/// - Create new reviews (with authorization checks)
/// - Delete reviews (only by review owner or admin)
/// - Calculate average rating for a product
/// 
/// DESIGN DECISIONS:
/// - Uses mapper to convert between entities and DTOs (consistency with other services)
/// - Validates product exists before allowing review operations
/// - Enforces that only the review creator can delete their review
/// - Returns aggregate rating for product display
/// </summary>
internal sealed class ReviewService : IReviewService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public ReviewService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Gets all reviews for a product
    /// 
    /// FLOW:
    /// 1. Verify product exists
    /// 2. Fetch all reviews for the product (ordered by newest first)
    /// 3. Map to DTOs
    /// 4. Return collection
    /// 
    /// WHY NO TRACKING: This is a read-only operation, so we use trackChanges: false for performance
    /// </summary>
    public async Task<IEnumerable<ReviewDto>> GetProductReviewsAsync(Guid productId)
    {
        _logger.LogInfo($"Fetching reviews for product {productId}");

        // Verify product exists
        var product = await _repository.ProductRepository.GetProductByIdAsync(productId, trackChanges: false);
        if (product == null)
            throw new ProductNotFoundException(productId);

        // Get reviews from repository
        var reviews = await _repository.ReviewRepository.GetProductReviewsAsync(productId, trackChanges: false);

        // Map to DTOs
        var reviewDtos = _mapper.Map<IEnumerable<ReviewDto>>(reviews);

        _logger.LogInfo($"Retrieved {reviews.Count()} reviews for product {productId}");
        return reviewDtos;
    }

    /// <summary>
    /// Creates a new review for a product (EC-007 requirement)
    /// 
    /// FLOW:
    /// 1. Verify product exists
    /// 2. Map DTO to Review entity
    /// 3. Set product ID and user ID
    /// 4. Create in repository
    /// 5. Save to database
    /// 6. Return created review as DTO
    /// 
    /// WHY THIS VALIDATION:
    /// - Prevents reviews for non-existent products
    /// - Links review to both product and authenticated user
    /// - Only logged-in users can submit reviews (userId is required)
    /// </summary>
    public async Task<ReviewDto> CreateReviewAsync(Guid productId, ReviewForCreationDto reviewForCreationDto, string userId)
    {
        _logger.LogInfo($"Creating review for product {productId} by user {userId}");

        // Validate input
        if (string.IsNullOrWhiteSpace(userId))
            throw new IdParametersBadRequestException("User ID is required to create a review");

        // Verify product exists
        var product = await _repository.ProductRepository.GetProductByIdAsync(productId, trackChanges: false);
        if (product == null)
            throw new ProductNotFoundException(productId);
        
        var hasPurchased = await _repository.OrderRepository.UserHasPurchasedProductAsync(userId, productId);
        if (!hasPurchased)
        {
            _logger.LogWarn($"User {userId} has not purchased product {productId}");
            throw new ProductNotPurchasedBadRequest($"User {userId} has not purchased product {productId}");
        }
        
        // Check if user has already reviewed this product
        var existingReview = await _repository.ReviewRepository.GetReviewByUserIdAsync(productId, userId, trackChanges: false);
        if (existingReview != null)
        {
            _logger.LogWarn($"User {userId} has already reviewed product {productId}");
            throw new ReviewAlreadyExistsBadRequest($"User {userId} has already reviewed product {productId}");
        }

        // Map DTO to entity
        var review = _mapper.Map<Review>(reviewForCreationDto);
        // review.Id = Guid.NewGuid();
        // review.CreatedAt = DateTime.UtcNow;
        review.UserId = userId;

        _logger.LogInfo($"Adding review to repository. Rating: {review.Rating}, Comment length: {review.Comment?.Length ?? 0}");

        // Create in repository (with productId)
        _repository.ReviewRepository.CreateReview(productId, review);

        // Save to database
        await _repository.SaveAsync();

        _logger.LogInfo($"Review {review.Id} created successfully");

        // Map back to DTO for response
        var reviewDtoResponse = _mapper.Map<ReviewDto>(review);
        return reviewDtoResponse;
    }

    /// <summary>
    /// Gets the average rating for a product (EC-007 requirement)
    /// 
    /// FLOW:
    /// 1. Verify product exists
    /// 2. Call repository to calculate average
    /// 3. Return average rating
    /// 
    /// WHY REPOSITORY CALL:
    /// - Calculation happens in database for performance (no N+1 queries)
    /// - Repository handles the LINQ aggregation
    /// - Returns 0 if no reviews exist
    /// </summary>
    public async Task<decimal> GetAverageRatingAsync(Guid productId)
    {
        _logger.LogInfo($"Calculating average rating for product {productId}");

        // Verify product exists
        var product = await _repository.ProductRepository.GetProductByIdAsync(productId, trackChanges: false);
        if (product == null)
            throw new ProductNotFoundException(productId);

        // Get average from repository
        var averageRating = await _repository.ReviewRepository.GetAverageRatingAsync(productId);

        _logger.LogInfo($"Average rating for product {productId}: {averageRating}");
        return averageRating;
    }

    /// <summary>
    /// Deletes a review (only by review owner)
    /// 
    /// FLOW:
    /// 1. Get the review
    /// 2. Verify review exists
    /// 3. Verify current user is the review owner
    /// 4. Delete from repository
    /// 5. Save to database
    /// 
    /// WHY AUTHORIZATION CHECK:
    /// - Only the user who created the review can delete it
    /// - Prevents malicious users from deleting other people's reviews
    /// - Security best practice for resource ownership
    /// </summary>
    public async Task DeleteReviewAsync(Guid reviewId, string userId)
    {
        _logger.LogInfo($"Attempting to delete review {reviewId} by user {userId}");

        // Get the review
        var review = await _repository.ReviewRepository.GetReviewAsync(reviewId, trackChanges: false);
        if (review == null)
            throw new ReviewNotFoundException(reviewId);

        // Verify user owns this review
        if (review.UserId != userId)
        {
            _logger.LogWarn($"User {userId} attempted to delete review {reviewId} owned by {review.UserId}");
            throw new ReviewBadRequestException("You can only delete your own reviews");
        }

        _logger.LogInfo($"Review {reviewId} belongs to user. Proceeding with deletion");

        // Delete from repository
        _repository.ReviewRepository.DeleteReview(review);

        // Save to database
        await _repository.SaveAsync();

        _logger.LogInfo($"Review {reviewId} deleted successfully");
    }
}