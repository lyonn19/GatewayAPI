using GatewayApi.Common.Results;
using GatewayApi.Features.Products.Models;

namespace GatewayApi.Features.Products.Services;

/// <summary>
/// Service interface for product operations.
/// </summary>
public interface IProductService
{
    Task<Result<IEnumerable<Product>>> GetAllProductsAsync();
    Task<Result<Product>> GetProductByIdAsync(Guid id);
    Task<Result<Product>> CreateProductAsync(CreateProductRequest request);
}
