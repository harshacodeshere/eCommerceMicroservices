using BusinessLogicLayer.DTO;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Polly.Bulkhead;
using System.Net.Http.Json;
using System.Text.Json;

namespace BusinessLogicLayer.HttpClients;

public class ProductMicroserviceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductMicroserviceClient> _logger;
    private readonly IDistributedCache _distributedCache;
    public ProductMicroserviceClient(HttpClient httpClient, ILogger<ProductMicroserviceClient> logger, IDistributedCache distributedCache)
    {
        _httpClient = httpClient;
        _logger = logger;
        _distributedCache = distributedCache;
    }

    public async Task<ProductDTO?> GetProductByProductID(Guid productID)
    {
        try
        {
            string cacheKey = $"product:{productID}";
            string? cachedProduct = await _distributedCache.GetStringAsync(cacheKey);

            if (cachedProduct != null)
            {
                _logger.LogInformation("Product retrieved from cache.");
                return JsonSerializer.Deserialize<ProductDTO>(cachedProduct);
            }

            HttpResponseMessage response = await _httpClient.GetAsync($"/gateway/products/search/product-id/{productID}");
            if (response.IsSuccessStatusCode)
            {
                var product = await response.Content.ReadFromJsonAsync<ProductDTO>();
                if (product == null)
                {
                    throw new HttpRequestException("Failed to deserialize product data", null, System.Net.HttpStatusCode.InternalServerError);
                }

                string productJson = JsonSerializer.Serialize(product);
                DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(300))
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(100));

                string cacheKeyToWrite = $"product:{productID}";
                
                await _distributedCache.SetStringAsync(cacheKeyToWrite, productJson, options);

                return product;
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null; // Product not found
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    throw new HttpRequestException("Bad request", null, System.Net.HttpStatusCode.BadRequest);
                }
                else
                {
                    throw new HttpRequestException($"Http request failed with status code {response.StatusCode}");
                }
            }
        }
        catch (BulkheadRejectedException ex)
        {
            _logger.LogError(ex, "Bulkhead isolation blocks the request since the request queue is full");

            return new ProductDTO(
              ProductID: Guid.NewGuid(),
              ProductName: "Temporarily Unavailable (Bulkhead)",
              Category: "Temporarily Unavailable (Bulkhead)",
              UnitPrice: 0,
              QuantityInStock: 0);
        }
    }
}
