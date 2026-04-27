using Entities.Models;

namespace Contracts;

public interface ICartRepository
{
    Task<Cart?> GetUserActiveCartAsync(string userId, bool trackChanges);
    Task<Cart?> GetCartByIdAsync(Guid cartId, bool trackChanges);
    void CreateCart(Cart cart);
    void UpdateCart(Cart cart);
    void DeleteCart(Cart cart);
}