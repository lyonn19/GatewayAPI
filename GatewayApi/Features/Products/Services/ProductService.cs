using GatewayApi.Common.Results;
using GatewayApi.Features.Products.Models;

namespace GatewayApi.Features.Products.Services;

/// <summary>
/// Service for managing products via downstream API calls.
/// </summary>
public class ProductService : IProductService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductService> _logger;
    private readonly string _downstreamApiBaseUrl;

    public ProductService(
        HttpClient httpClient,
        ILogger<ProductService> logger,
        IConfiguration configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _downstreamApiBaseUrl = configuration["DownstreamApi:BaseUrl"]
            ?? throw new ArgumentException("DownstreamApi:BaseUrl configuration is required");

        _httpClient.BaseAddress = new Uri(_downstreamApiBaseUrl);
    }

    public async Task<Result<IEnumerable<Product>>> GetAllProductsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/products");
            return await HandleApiResponse<IEnumerable<Product>>(response);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to call downstream API for GetAllProducts");
            return Result<IEnumerable<Product>>.Failure("Failed to retrieve products from downstream service", 500);
        }
    }

    public async Task<Result<Product>> GetProductByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/products/{id}");
            return await HandleApiResponse<Product>(response);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to call downstream API for GetProductById");
            return Result<Product>.Failure("Failed to retrieve product from downstream service", 500);
        }
    }

    public async Task<Result<Product>> CreateProductAsync(CreateProductRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/products", request);
            return await HandleApiResponse<Product>(response);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to call downstream API for CreateProduct");
            return Result<Product>.Failure("Failed to create product in downstream service", 500);
        }
    }

    private async Task<Result<T>> HandleApiResponse<T>(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadFromJsonAsync<T>();
            return content != null
                ? Result<T>.Success(content)
                : Result<T>.Failure("Empty response from downstream service", 204);
        }

        return response.StatusCode switch
        {
            System.Net.HttpStatusCode.NotFound => Result<T>.NotFound(typeof(T).Name),
            System.Net.HttpStatusCode.Unauthorized => Result<T>.Failure("Unauthorized access to downstream service", 401),
            System.Net.HttpStatusCode.Forbidden => Result<T>.Failure("Forbidden access to downstream service", 403),
            System.Net.HttpStatusCode.BadRequest => Result<T>.Failure("Bad request to downstream service", 400),
            _ => Result<T>.Failure($"Downstream service error: {response.StatusCode}", (int)response.StatusCode)
        };
    }
}
