using AutoMapper;
    using Contracts;
    using Entities.Enums;
    using Entities.Exceptions;
    using Entities.Models;
    using Service.Contracts;
    using Shared.DataTransferObjects;
    
    namespace Service;
    
    /// <summary>
    /// CartService handles shopping cart business logic.
    /// 
    /// KEY RESPONSIBILITIES:
    /// - Get/create user's active cart
    /// - Return cart with calculated total (not stored in DB)
    /// - Clear cart items
    /// 
    /// DOES NOT handle individual items - that's CartItemService's job
    /// </summary>
    internal sealed class CartService : ICartService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
    
        public CartService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
    
        /// <summary>
        /// Gets user's active cart with all items
        /// Calculates total from CartItems (not stored in DB)
        /// </summary>
        public async Task<CartDto> GetUserCartAsync(string userId)
        {
            _logger.LogInfo($"Getting cart for user {userId}");
    
            // Get user's active cart with all items
            var cart = await _repository.CartRepository.GetUserActiveCartAsync(userId, trackChanges: false);
    
            // Return empty cart if doesn't exist (user hasn't added anything yet)
            if (cart == null)
            {
                _logger.LogInfo($"No active cart found for user {userId}, returning empty cart");
                return new CartDto
                {
                    Items = new List<CartItemDto>(),
                    TotalAmount = 0
                };
            }
    
            // Convert to DTO with calculated total
            return MapCartToDto(cart);
        }
    
        /// <summary>
        /// Gets or creates user's active cart
        /// Used internally when performing cart operations
        /// </summary>
        public async Task<CartDto> EnsureUserCartAsync(string userId)
        {
            _logger.LogInfo($"Ensuring active cart exists for user {userId}");
    
            var cart = await _repository.CartRepository.GetUserActiveCartAsync(userId, trackChanges: true);
    
            // Cart doesn't exist, create new one
            if (cart == null)
            {
                cart = new Cart
                {
                    cartId = Guid.NewGuid(),
                    UserId = userId,
                    Status = CartStatus.Active,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CartItems = new List<CartItem>()
                };
    
                _repository.CartRepository.CreateCart(cart);
                await _repository.SaveAsync();
                _logger.LogInfo($"Created new cart for user {userId}");
            }
    
            // Initialize CartItems if null
            cart.CartItems ??= new List<CartItem>();
    
            return MapCartToDto(cart);
        }
    
        /// <summary>
        /// Clears all items from user's cart but keeps the cart record
        /// Useful for "empty cart" button on frontend
        /// </summary>
        public async Task ClearCartAsync(string userId)
        {
            _logger.LogInfo($"Clearing cart for user {userId}");

            var cart = await _repository.CartRepository.GetUserActiveCartAsync(userId, trackChanges: true);

            if (cart == null)
            {
                throw new CartNotFoundException($"No active cart found for user {userId}");
            }

            // Delete all items in cart - each RemoveFromCart call marks the CartItem
            // entity for deletion in the DbContext. When SaveAsync is called,
            // EF Core will issue DELETE statements, permanently removing the
            // CartItem rows from the database table.
            if (cart.CartItems?.Any() == true)
            {
                foreach (var item in cart.CartItems.ToList())
                {
                    _repository.CartItemRepository.DeleteCartItem(item);
                }
            }

            cart.UpdatedAt = DateTime.UtcNow;
            _repository.CartRepository.UpdateCart(cart);
            await _repository.SaveAsync();

            _logger.LogInfo($"Cart cleared for user {userId}");
        }

        /// <summary>
        /// Deletes user's cart (and all CartItems)
        /// </summary>
        public async Task DeleteCartAsync(string userId)
        {
            _logger.LogInfo($"Deleting cart for user {userId}");

            var cart = await _repository.CartRepository.GetUserActiveCartAsync(userId, trackChanges: true);

            if (cart == null)
            {
                throw new CartNotFoundException($"No active cart found for user {userId}");
            }

            _repository.CartRepository.DeleteCart(cart);
            await _repository.SaveAsync();

            _logger.LogInfo($"Cart deleted for user {userId}");
        }
        
        
        
        
        
        /// <summary>
        /// Converts Cart entity to CartDto using mapper
        /// Calculates total from CartItems (single source of truth)
        /// </summary>
        public CartDto MapCartToDto(Cart cart)
        {
            var items = cart.CartItems?
                .Select(ci => new CartItemDto
                {
                    Id = ci.Id,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product?.Name,
                    ProductPrice = ci.Product?.Price ?? 0,
                    Quantity = ci.Quantity,
                    TotalPrice = (ci.Product?.Price ?? 0) * ci.Quantity
                })
                .ToList() ?? new List<CartItemDto>();
    
            // Calculate total from items
            var totalAmount = items.Sum(i => i.TotalPrice);
    
            return new CartDto
            {
                // Id = cart.Id
                Items = items,
                TotalAmount = totalAmount
            };
        }
    }