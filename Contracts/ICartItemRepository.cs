using Entities.Models;

namespace Contracts;

public interface ICartItemRepository
{
    Task<IEnumerable<CartItem>> GetCartItemsAsync(Guid cartId, bool trackChanges);
    Task<CartItem?> GetCartItemAsync(Guid cartId, Guid productId, bool trackChanges);
    void CreateCartItem(CartItem cartItem);
    void UpdateCartItem(CartItem cartItem);
    void DeleteCartItem(CartItem cartItem);
}