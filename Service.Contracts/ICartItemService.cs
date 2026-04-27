using Shared.DataTransferObjects;

namespace Service.Contracts;

/// <summary>
/// ICartItemService defines operations for items within a cart.
/// Handles adding, updating, and removing products from cart.
/// 
/// Cart management is handled by ICartService for separation of concerns.
/// </summary>
public interface ICartItemService
{
    /// <summary>
    /// Adds a product to user's cart
    /// - Creates cart if it doesn't exist
    /// - If product already in cart, increases quantity instead
    /// - Returns updated cart with all items
    /// </summary>
    Task<CartDto> AddToCartItemToCartAsync(string userId, AddToCartDto AddToCartDto);
    
    /// <summary>
    /// Updates quantity of an item already in cart
    /// Throws exception if item not found
    /// </summary>
    Task<CartDto> UpdateCartItemAsync(string userId, UpdateCartItemDto UpdateCartItemDto);
    
    /// <summary>
    /// Removes a product from user's cart
    /// Returns updated cart
    /// </summary>
    Task<CartDto> DeleteCartItemAsync(string userId, Guid productId);
    
    /// <summary>
    /// Increases quantity of an item already in cart
    /// Throws exception if item not found
    /// </summary>
    Task<CartDto> IncreaseCartItemQuantityAsync(string userId, Guid productId);
    
    /// <summary>
    /// Decreases quantity of an item already in cart
    /// Throws exception if item not found
    /// </summary>
    Task<CartDto> DecreaseCartItemQuantityAsync(string userId, Guid productId);
}