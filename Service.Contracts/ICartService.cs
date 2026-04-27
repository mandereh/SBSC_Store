using Shared.DataTransferObjects;

namespace Service.Contracts;

/// <summary>
/// ICartService defines operations for shopping cart management.
/// Handles cart lifecycle: creation, retrieval, and status management.
/// 
/// CartItems are managed through ICartItemService for separation of concerns.
/// </summary>
public interface ICartService
{
    /// <summary>
    /// Gets user's active cart with all items and calculated total
    /// Returns cart even if empty (no items)
    /// </summary>
    Task<CartDto> GetUserCartAsync(string userId);
    
    /// <summary>
    /// Gets or creates a user's active cart
    /// Called internally when user performs cart operations
    /// </summary>
    Task<CartDto> EnsureUserCartAsync(string userId);
    
    /// <summary>
    /// Clears all items from user's cart (empties but doesn't delete cart)
    /// </summary>
    Task ClearCartAsync(string userId);
    
    /// <summary>
    /// Deletes user's cart and all items
    /// </summary>
    Task DeleteCartAsync(string userId);
}