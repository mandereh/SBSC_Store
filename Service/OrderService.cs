using AutoMapper;
using Contracts;
using Entities.Enums;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

internal sealed class OrderService : IOrderService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;
    private readonly IPaymentServiceFactory _paymentServiceFactory;
    private readonly UserManager<User> _userManager;

    public OrderService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper,
        IPaymentServiceFactory paymentServiceFactory, UserManager<User> userManager)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
        _paymentServiceFactory = paymentServiceFactory;
        _userManager = userManager;
    }

   
    public async Task<PlaceOrderResponseDto> PlaceOrderAsync(string userId)
    {
        _logger.LogInfo($"Placing order for user {userId}");

        var cart = await _repository.CartRepository.GetUserActiveCartAsync(userId, trackChanges: false);
        if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
            throw new EmptyCartBadRequest("Your cart is empty. Add items before placing an order.");

        var totalAmount = cart.CartItems.Sum(ci => ci.Quantity * (ci.Product?.Price ?? 0));

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || string.IsNullOrEmpty(user.Email))
            throw new UserNotFoundException("User email not found. Cannot initialize payment.");

        var orderNumber = GenerateOrderNumber();
        var paymentReference = $"{orderNumber}-{Guid.NewGuid().ToString("N").Substring(0, 8)}";

        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = orderNumber,
            UserId = userId,
            TotalAmount = totalAmount,
            Status = OrderStatus.Pending,
            PaymentReference = paymentReference,
            PaymentProvider = PaymentServiceType.Paystack.ToString(),
            OrderItems = cart.CartItems.Select(ci => new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = ci.ProductId,
                ProductName = ci.Product?.Name,
                ProductPrice = ci.Product?.Price ?? 0,
                Quantity = ci.Quantity,
                TotalPrice = ci.Quantity * (ci.Product?.Price ?? 0)
            }).ToList()
        };

        _repository.OrderRepository.CreateOrder(order);
        await _repository.SaveAsync();

        var paymentService = _paymentServiceFactory.Create(PaymentServiceType.Paystack);
        var paymentResult = await paymentService.InitializePaymentAsync(totalAmount, user.Email, paymentReference);

        _logger.LogInfo($"Order {orderNumber} placed with Paystack reference {paymentReference}");

        return new PlaceOrderResponseDto
        {
            OrderNumber = orderNumber,
            PaymentReference = paymentReference,
            PaymentUrl = paymentResult.AuthorizationUrl,
            TotalAmount = totalAmount,
            Status = order.Status.ToString()
        };
    }

    public async Task<OrderDto> VerifyPaymentAndCompleteOrderAsync(string paymentReference)
    {
        _logger.LogInfo($"Verifying payment for reference {paymentReference}");

        var order = await _repository.OrderRepository.GetOrderByPaymentReferenceAsync(paymentReference, trackChanges: true);
        if (order == null)
            throw new OrderNotFoundException($"Order with reference {paymentReference} not found.");

        if (order.Status != OrderStatus.Pending)
            throw new PaymentFailedBadRequest($"Order is already in status: {order.Status}.");

        var paymentService = _paymentServiceFactory.Create(Enum.Parse<PaymentServiceType>(order.PaymentProvider!));
        var verification = await paymentService.VerifyPaymentAsync(paymentReference);

        if (!verification.Success)
        {
            order.Status = OrderStatus.Failed;
            _repository.OrderRepository.UpdateOrder(order);
            await _repository.SaveAsync();
            throw new PaymentFailedBadRequest($"Payment verification failed: {verification.Message}");
        }

        order.Status = OrderStatus.Paid;
        _repository.OrderRepository.UpdateOrder(order);

        var cart = await _repository.CartRepository.GetUserActiveCartAsync(order.UserId, trackChanges: true);
        if (cart != null)
        {
            _repository.CartRepository.DeleteCart(cart);
        }

        await _repository.SaveAsync();

        _logger.LogInfo($"Order {order.OrderNumber} confirmed and cart cleared for user {order.UserId}");

        return _mapper.Map<OrderDto>(order);
    }
    
    public async Task<OrderDto> ProcessWebhookAsync(string rawBody, string? signature)
    {
        _logger.LogInfo("Processing payment webhook");

        var paymentService = _paymentServiceFactory.Create(PaymentServiceType.Paystack);

        if (!paymentService.VerifyWebhookSignature(rawBody, signature ?? string.Empty))
            throw new InvalidWebhookSignatureBadRequest();

        var reference = paymentService.ExtractWebhookReference(rawBody);
        if (string.IsNullOrEmpty(reference))
            throw new PaymentFailedBadRequest("Could not extract payment reference from webhook payload.");

        return await VerifyPaymentAndCompleteOrderAsync(reference);
    }

    public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId)
    {
        var orders = await _repository.OrderRepository.GetOrdersByUserAsync(userId, trackChanges: false);
        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public async Task<OrderDto> GetOrderByNumberAsync(string orderNumber, string userId)
    {
        var order = await _repository.OrderRepository.GetOrderByNumberAsync(orderNumber, trackChanges: false);
        if (order == null || order.UserId != userId)
            throw new OrderNotFoundException(orderNumber);
        return _mapper.Map<OrderDto>(order);
    }

    private string GenerateOrderNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpperInvariant();
        return $"ORD-{timestamp}-{random}";
    }
}