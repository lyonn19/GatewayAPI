using GatewayApi.Common.Results;
using GatewayApi.Features.Products.Models;
using GatewayApi.Features.Products.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GatewayApi.Features.Products.Endpoints;

[ApiController]
[Route("api/[controller]")]
[Tags("Products")]
//[Authorize] // Require authentication for all endpoints
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductController> _logger;

    public ProductController(IProductService productService, ILogger<ProductController> logger)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    //[Authorize(Roles = "User,Admin")] // Users and Admins can view products
    public async Task<IActionResult> GetAllProducts(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all products");

        var result = await _productService.GetAllProductsAsync();

        if (result.IsSuccess)
        {
            _logger.LogInformation("Successfully retrieved {ProductCount} products", result.Value?.Count() ?? 0);
            return Ok(result);
        }

        _logger.LogWarning("Failed to get products: {Error}", result.Error);
        return Problem(statusCode: result.StatusCode, detail: result.Error);
    }

    [HttpGet("{id:guid}")]
    //[Authorize(Roles = "User,Admin")] // Users and Admins can view specific products
    public async Task<IActionResult> GetProductById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting product with ID: {ProductId}", id);

        var result = await _productService.GetProductByIdAsync(id);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Successfully retrieved product with ID: {ProductId}", id);
            return Ok(result);
        }

        if (result.StatusCode == 404)
        {
            _logger.LogWarning("Product {ProductId} not found", id);
            return Problem(statusCode: 404, detail: "Product not found");
        }

        _logger.LogWarning("Failed to get product {ProductId}: {Error}", id, result.Error);
        return Problem(statusCode: result.StatusCode, detail: result.Error);
    }

    [HttpPost]
    //[Authorize(Roles = "Admin")] // Only Admins can create products
    public async Task<IActionResult> CreateProduct(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirst("sub")?.Value ?? "unknown";
        _logger.LogInformation("User {UserId} is creating a new product: {ProductName}", userId, request.Name);

        var result = await _productService.CreateProductAsync(request);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Successfully created product {ProductId} by user {UserId}", result.Value?.Id, userId);
            return CreatedAtAction(nameof(GetProductById), new { id = result.Value?.Id }, result);
        }

        _logger.LogWarning("Failed to create product by user {UserId}: {Error}", userId, result.Error);
        return Problem(statusCode: result.StatusCode, detail: result.Error);
    }
}
