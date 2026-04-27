using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Presentation.Controllers;

[Route("api/cart/{cartId}/items")]
[ApiController]
public class CartItemsController : ControllerBase
{
    private readonly IServiceManager _serviceManager;
    public CartItemsController(IServiceManager serviceManager) => _serviceManager = serviceManager;
    
    
    
    
    
    

    
}