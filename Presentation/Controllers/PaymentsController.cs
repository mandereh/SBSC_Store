using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Presentation.Controllers;

[Route("api/payments")]
[ApiController]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public PaymentsController(IServiceManager serviceManager) => _serviceManager = serviceManager;

    /// <summary>
    /// Verifies a payment and completes the order.
    /// </summary>
    /// <param name="verifyPaymentRequestDto">The request containing the payment reference.</param>
    /// <returns>An IActionResult indicating the result of the payment verification and order completion.</returns>
    [HttpPost("verify", Name = "VerifyPayment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> VerifyPayment([FromBody] VerifyPaymentRequestDto verifyPaymentRequestDto)
    {
        if (verifyPaymentRequestDto == null || string.IsNullOrEmpty(verifyPaymentRequestDto.Reference))
            return BadRequest("Payment reference is required.");
    
        var result = await _serviceManager.OrderService.VerifyPaymentAndCompleteOrderAsync(verifyPaymentRequestDto.Reference);
        return Ok(result);
    }
    
    /// <summary>
    /// Handles the Paystack payment callback (browser redirect after payment).
    /// </summary>
    /// <param name="trxref">Transaction reference from Paystack.</param>
    /// <param name="reference">Payment reference from Paystack.</param>
    /// <returns>Redirects to a success or failure page, or returns verification result.</returns>
    [HttpGet("callback", Name = "VerifyPaymentCallback")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyPaymentCallback([FromQuery] string trxref, [FromQuery] string reference)
    {
        if (string.IsNullOrEmpty(reference))
            return BadRequest("Payment reference is required.");

        // Call your service to verify the payment and complete the order
        // This is the same logic you might have in your [HttpPost("verify")] if it's meant for frontend-initiated verification
        var result = await _serviceManager.OrderService.VerifyPaymentAndCompleteOrderAsync(reference);
        
        // You might want to redirect the user to a frontend success/failure page here
        // For now, returning the result directly.
        return Ok(result);
    }
    
    
    /// <summary>
    /// Handles the Paystack webhook.
    /// </summary>
    /// <returns>An IActionResult indicating the result of the webhook processing.</returns>
    [HttpPost("webhook")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Webhook()
    {
        using var reader = new StreamReader(Request.Body);
        var rawBody = await reader.ReadToEndAsync();
        var signature = Request.Headers["x-paystack-signature"].FirstOrDefault();

        try
        {
            await _serviceManager.OrderService.ProcessWebhookAsync(rawBody, signature);
            return Ok();
        }
        catch (Entities.Exceptions.InvalidWebhookSignatureBadRequest)
        {
            return Unauthorized();
        }
        catch (Entities.Exceptions.OrderNotFoundException)
        {
            // Return 200 so Paystack doesn't retry for unknown references
            return Ok();
        }
        catch (Entities.Exceptions.PaymentFailedBadRequest)
        {
            // Return 200 so Paystack doesn't retry for already-processed/failed payments
            return Ok();
        }
    }
}