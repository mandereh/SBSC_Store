using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository;

/// <summary>
/// CartItemRepository handles all database operations for items in a cart.
/// Each CartItem represents one product with a quantity in a specific cart.
/// </summary>
public class CartItemRepository : RepositoryBase<CartItem>, ICartItemRepository
{
    public CartItemRepository(RepositoryContext repositoryContext) : base(repositoryContext) { }

    /// <summary>
    /// Gets all items in a specific cart
    /// 
    /// WHY THIS METHOD:
    /// - Display all products in user's cart with prices and quantities
    /// - Used to calculate running total (EC-006 requirement)
    /// - Includes Product details for rendering on frontend
    /// </summary>
    public async Task<IEnumerable<CartItem>> GetCartItemsAsync(Guid cartId, bool trackChanges)
    {
        return await FindByCondition(item => item.CartId == cartId, trackChanges)
            .Include(item => item.Product) // Load product for display (name, price, image)
            .OrderBy(item => item.CreatedAt) // Show in order added
            .ToListAsync();
    }

    /// <summary>
    /// Gets a specific item in a cart
    /// 
    /// WHY THIS METHOD:
    /// - Check if product already exists in cart before adding
    /// - If exists: update quantity instead of creating duplicate
    /// - If not exists: create new CartItem
    /// </summary>
    public async Task<CartItem?> GetCartItemAsync(Guid cartId, Guid productId, bool trackChanges)
    {
        return await FindByCondition(
                item => item.CartId == cartId && item.ProductId == productId,
                trackChanges)
            .SingleOrDefaultAsync(); // Unique constraint: one product per cart
    }

    /// <summary>
    /// Adds a new item to cart
    /// </summary>
    public void CreateCartItem(CartItem cartItem) => Create(cartItem);

    /// <summary>
    /// Updates a cart item (e.g., quantity change)
    /// </summary>
    public void UpdateCartItem(CartItem cartItem) => Update(cartItem);

    /// <summary>
    /// Removes an item from cart
    /// </summary>
    public void DeleteCartItem(CartItem cartItem) => Delete(cartItem);
}