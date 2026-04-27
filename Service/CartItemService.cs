using AutoMapper;
        using Contracts;
        using Entities.Enums;
        using Entities.Exceptions;
        using Entities.Models;
        using Service.Contracts;
        using Shared.DataTransferObjects;
        
        namespace Service;
        
        /// <summary>
        /// CartItemService handles operations on items within a cart.
        /// 
        /// KEY RESPONSIBILITIES:
        /// - Add product to cart (or increase quantity if already exists)
        /// - Update item quantity
        /// - Remove item from cart
        /// 
        /// DOES NOT depend on CartService - gets helper method to return updated cart
        /// Each method handles its own cart retrieval and returns updated cart
        /// </summary>
        internal sealed class CartItemService : ICartItemService
        {
            private readonly IRepositoryManager _repository;
            private readonly ILoggerManager _logger;
            private readonly IMapper _mapper;
        
            public CartItemService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
            {
                _repository = repository;
                _logger = logger;
                _mapper = mapper;
            }
            
        
            /// <summary>
            /// Adds a product to user's cart
            /// 
            /// FLOW:
            /// 1. Ensure cart exists (create if needed)
            /// 2. Verify product exists
            /// 3. Check if product already in cart
            ///    - If YES: increase quantity
            ///    - If NO: add new CartItem
            /// 4. Save to DB (repository layer)
            /// 5. Return updated cart
            /// </summary>
            public async Task<CartDto> AddToCartItemToCartAsync(string userId, AddToCartDto addToCartDto)
            {
                _logger.LogInfo($"Adding product {addToCartDto.ProductId} to cart for user {userId}. Quantity: {addToCartDto.Quantity}");
        
                if (addToCartDto.Quantity <= 0)
                    throw new CartBadRequestException("Quantity must be greater than 0");
        
                // Ensure cart exists (create if needed)
                var cart = await _repository.CartRepository.GetUserActiveCartAsync(userId, trackChanges: true);
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
                    // Save cart to database before adding items
                    await _repository.SaveAsync();
                    _logger.LogInfo($"Created new cart for user {userId}");
                }
        
                // Verify product exists
                var product = await _repository.ProductRepository.GetProductByIdAsync(addToCartDto.ProductId, trackChanges: false);
                if (product == null)
                    throw new ProductNotFoundException(addToCartDto.ProductId);
        
                // Check if product already in cart
                var existingItem = await _repository.CartItemRepository.GetCartItemAsync(cart.cartId, addToCartDto.ProductId, trackChanges: true);
        
                if (existingItem != null)
                {
                    // Product already in cart, increase quantity
                    existingItem.Quantity += addToCartDto.Quantity;
                    _logger.LogInfo($"Updated quantity for product {addToCartDto.ProductId}. New quantity: {existingItem.Quantity}");
                    _repository.CartItemRepository.UpdateCartItem(existingItem);
                }
                else
                {
                    // New product, add to cart
                    var cartItem = new CartItem
                    {
                        Id = Guid.NewGuid(),
                        CartId = cart.cartId,
                        ProductId = addToCartDto.ProductId,
                        Quantity = addToCartDto.Quantity,
                        CreatedAt = DateTime.UtcNow
                    };
        
                    _logger.LogInfo($"Added new product {addToCartDto.ProductId} to cart with quantity {addToCartDto.Quantity}");
                    _repository.CartItemRepository.CreateCartItem(cartItem);
                }
        
                cart.UpdatedAt = DateTime.UtcNow;
                _repository.CartRepository.UpdateCart(cart);
                
                // SAVE IN REPOSITORY LAYER - Not in service
                await _repository.SaveAsync();
        
                // Return updated cart
                return await GetUpdatedCartDto(userId);
            }
        
            /// <summary>
            /// Updates quantity of an item already in cart
            /// 
            /// FLOW:
            /// 1. Get user's cart
            /// 2. Find the CartItem
            /// 3. Update quantity (or remove if quantity = 0)
            /// 4. Save to DB (repository layer)
            /// 5. Return updated cart
            /// </summary>
            public async Task<CartDto> UpdateCartItemAsync(string userId, UpdateCartItemDto updateCartItemDto)
            {
                _logger.LogInfo($"Updating cart item. User: {userId}, Product: {updateCartItemDto.ProductId}, New Quantity: {updateCartItemDto.Quantity}");
        
                if (updateCartItemDto.Quantity < 0)
                    throw new CartBadRequestException("Quantity cannot be negative");
        
                // Get user's cart
                var cart = await _repository.CartRepository.GetUserActiveCartAsync(userId, trackChanges: true);
                if (cart == null)
                    throw new CartNotFoundException($"No active cart found for user {userId}");
        
                // Find the item in cart
                var cartItem = await _repository.CartItemRepository.GetCartItemAsync(cart.cartId, updateCartItemDto.ProductId, trackChanges: true);
                if (cartItem == null)
                    throw new CartBadRequestException($"Product {updateCartItemDto.ProductId} not found in cart");
        
                // If quantity is 0, remove item instead of updating
                if (updateCartItemDto.Quantity == 0)
                {
                    _logger.LogInfo($"Removing product {updateCartItemDto.ProductId} from cart (quantity set to 0)");
                    _repository.CartItemRepository.DeleteCartItem(cartItem);
                }
                else
                {
                    // Update quantity
                    cartItem.Quantity = updateCartItemDto.Quantity;
                    _repository.CartItemRepository.UpdateCartItem(cartItem);
                }
        
                cart.UpdatedAt = DateTime.UtcNow;
                _repository.CartRepository.UpdateCart(cart);
                
                // SAVE IN REPOSITORY LAYER - Not in service
                await _repository.SaveAsync();
        
                // Return updated cart
                return await GetUpdatedCartDto(userId);
            }
        
            /// <summary>
            /// Removes a product from user's cart
            /// 
            /// FLOW:
            /// 1. Get user's cart
            /// 2. Find and delete the CartItem
            /// 3. Save to DB (repository layer)
            /// 4. Return updated cart
            /// </summary>
            public async Task<CartDto> DeleteCartItemAsync(string userId, Guid productId)
            {
                _logger.LogInfo($"Removing product {productId} from cart for user {userId}");
        
                // Get user's cart
                var cart = await _repository.CartRepository.GetUserActiveCartAsync(userId, trackChanges: true);
                if (cart == null)
                    throw new CartNotFoundException($"No active cart found for user {userId}");
        
                // Find the item in cart
                var cartItem = await _repository.CartItemRepository.GetCartItemAsync(cart.cartId, productId, trackChanges: true);
                if (cartItem == null)
                    throw new CartBadRequestException($"Product {productId} not found in cart");
        
                // Remove item
                _repository.CartItemRepository.DeleteCartItem(cartItem);
        
                cart.UpdatedAt = DateTime.UtcNow;
                _repository.CartRepository.UpdateCart(cart);
                
                //Delete CartItem from DB
                
                // SAVE IN REPOSITORY LAYER - Not in service
                await _repository.SaveAsync();
        
                _logger.LogInfo($"Product {productId} removed from cart");
        
                // Return updated cart
                return await GetUpdatedCartDto(userId);
            }


            public async Task<CartDto> IncreaseCartItemQuantityAsync(string userId, Guid productId)
            {
                _logger.LogInfo($"Increasing quantity of product {productId} in cart for user {userId}");
        
                // Get user's cart
                var cart = await _repository.CartRepository.GetUserActiveCartAsync(userId, trackChanges: true);
                if (cart == null)
                    throw new CartNotFoundException($"No active cart found for user {userId}");
        
                // Find the item in cart
                var cartItem = await _repository.CartItemRepository.GetCartItemAsync(cart.cartId, productId, trackChanges: true);
                if (cartItem == null)
                {
                    _logger.LogWarn($"Product {productId} not found in cart {cart.cartId} for user {userId}");
                    throw new CartBadRequestException($"Product {productId} not found in cart");
                }
        
                // Increase quantity
                cartItem.Quantity++;
                _repository.CartItemRepository.UpdateCartItem(cartItem);
        
                cart.UpdatedAt = DateTime.UtcNow;
                _repository.CartRepository.UpdateCart(cart);
                
                // SAVE IN REPOSITORY LAYER - Not in service
                await _repository.SaveAsync();
        
                _logger.LogInfo($"Increased quantity of product {productId} in cart. New quantity: {cartItem.Quantity}");
        
                // Return updated cart
                return await GetUpdatedCartDto(userId);
            }
            
            public async Task<CartDto> DecreaseCartItemQuantityAsync(string userId, Guid productId)
            {
                _logger.LogInfo($"Decreasing quantity of product {productId} in cart for user {userId}");
        
                // Get user's cart
                var cart = await _repository.CartRepository.GetUserActiveCartAsync(userId, trackChanges: true);
                if (cart == null)
                    throw new CartNotFoundException($"No active cart found for user {userId}");
        
                // Find the item in cart
                var cartItem = await _repository.CartItemRepository.GetCartItemAsync(cart.cartId, productId, trackChanges: true);
                if (cartItem == null)
                {
                    _logger.LogWarn($"Product {productId} not found in cart {cart.cartId} for user {userId}");
                    throw new CartBadRequestException($"Product {productId} not found in cart");
                }
        
                // Decrease quantity
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity--;
                    _repository.CartItemRepository.UpdateCartItem(cartItem);
                    _logger.LogInfo($"Decreased quantity of product {productId} in cart. New quantity: {cartItem.Quantity}");
                }
                else
                {
                    _logger.LogInfo($"Quantity of product {productId} is 1. Removing from cart.");
                    _repository.CartItemRepository.DeleteCartItem(cartItem);
                }
        
                cart.UpdatedAt = DateTime.UtcNow;
                _repository.CartRepository.UpdateCart(cart);
                
                // SAVE IN REPOSITORY LAYER - Not in service
                await _repository.SaveAsync();
        
                // Return updated cart
                return await GetUpdatedCartDto(userId);
            }
        
            /// <summary>
            /// Helper method: Gets updated cart and converts to DTO
            /// Used after any cart modification to return the updated state
            /// </summary>
            private async Task<CartDto> GetUpdatedCartDto(string userId)
            {
                var cart = await _repository.CartRepository.GetUserActiveCartAsync(userId, trackChanges: false);
        
                if (cart == null)
                {
                    return new CartDto
                    {
                        Items = new List<CartItemDto>(),
                        TotalAmount = 0
                    };
                }
        
                // Map CartItems to DTOs
                var items = cart.CartItems?
                    .Select(ci => _mapper.Map<CartItemDto>(ci))
                    .ToList() ?? new List<CartItemDto>();
        
                // Calculate total
                var totalAmount = items.Sum(i => i.TotalPrice);
        
                return new CartDto
                {
                    Items = items,
                    TotalAmount = totalAmount
                };
            }
        }
