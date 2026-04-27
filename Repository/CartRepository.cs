using Contracts;
using Entities.Enums;
using Entities.Models;
    using Microsoft.EntityFrameworkCore;
    
    namespace Repository;
    
    /// <summary>
    /// CartRepository handles all database operations for shopping carts.
    /// A Cart represents a shopping session for a user.
    /// 
    /// WHY THIS PATTERN:
    /// - Each user has ONE active cart at a time
    /// - Cart persists across sessions (user can browse on phone, continue on desktop)
    /// - Multiple cart statuses support abandoned cart recovery features
    /// - CartItems are managed by CartItemRepository (separation of concerns)
    /// </summary>
    public class CartRepository : RepositoryBase<Cart>, ICartRepository
    {
        public CartRepository(RepositoryContext repositoryContext) : base(repositoryContext) { }
    
        /// <summary>
        /// Gets a user's active shopping cart
        /// 
        /// WHY THIS METHOD:
        /// - Each user has ONE active cart at a time
        /// - Cart contains reference to all their current shopping items
        /// - Includes CartItems with Product details for display
        /// </summary>
        public async Task<Cart?> GetUserActiveCartAsync(string userId, bool trackChanges)
        {
            return await FindByCondition(
                    c => c.UserId == userId && c.Status == CartStatus.Active, 
                    trackChanges)
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync();
        }
    
        /// <summary>
        /// Gets a cart by ID (useful for admin operations or order processing)
        /// </summary>
        public async Task<Cart?> GetCartByIdAsync(Guid cartId, bool trackChanges)
        {
            return await FindByCondition(c => c.cartId == cartId, trackChanges)
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync();
        }
    
        /// <summary>
        /// Creates a new cart for a user
        /// 
        /// WHY THIS METHOD:
        /// - Called when user adds their first item (creates cart if doesn't exist)
        /// - Or when creating a new shopping session
        /// </summary>
        public void CreateCart(Cart cart) => Create(cart);
    
        /// <summary>
        /// Updates cart (usually status: Active → Abandoned → Recovered → Completed)
        /// </summary>
        public void UpdateCart(Cart cart) => Update(cart);
    
        /// <summary>
        /// Deletes a cart (and its items via cascade delete)
        /// </summary>
        public void DeleteCart(Cart cart) => Delete(cart);
    }