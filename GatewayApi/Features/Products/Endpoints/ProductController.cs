using GatewayApi.Common.Results;
using GatewayApi.Features.Products.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GatewayApi.Features.Products.Endpoints;

[ApiController]
[Route("api/[controller]")]
[Tags("Products")]
//[Authorize] // Require authentication for all endpoints
public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;
    private static readonly List<Product> _products = new();

    public ProductController(ILogger<ProductController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "User,Admin")] // Users and Admins can view products
    public async Task<IActionResult> GetAllProducts(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all products");

        var result = Result<IEnumerable<Product>>.Success(_products.AsReadOnly());

        _logger.LogInformation("Successfully retrieved {ProductCount} products", _products.Count);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        _logger.LogWarning("Failed to get products: {Error}", result.Error);
        return Problem(statusCode: result.StatusCode, detail: result.Error);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "User,Admin")] // Users and Admins can view specific products
    public async Task<IActionResult> GetProductById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting product with ID: {ProductId}", id);

        var product = _products.FirstOrDefault(p => p.Id == id);

        if (product == null)
        {
            _logger.LogWarning("Product {ProductId} not found", id);
            return Problem(statusCode: 404, detail: "Product not found");
        }

        _logger.LogInformation("Successfully retrieved product with ID: {ProductId}", id);
        var result = Result<Product>.Success(product);

        if (result.IsSuccess)
        {
            return Ok(result);
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

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock
        };

        _products.Add(product);

        _logger.LogInformation("Successfully created product {ProductId} by user {UserId}", product.Id, userId);

        var result = Result<Product>.Success(product);

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, result);
        }

        _logger.LogWarning("Failed to create product by user {UserId}: {Error}", userId, result.Error);
        return Problem(statusCode: result.StatusCode, detail: result.Error);
    }
}
