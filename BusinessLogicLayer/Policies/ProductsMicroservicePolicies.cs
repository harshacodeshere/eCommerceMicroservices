using BusinessLogicLayer.DTO;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Bulkhead;
using Polly.Fallback;
using Polly.Wrap;
using System.Net;
using System.Text;
using System.Text.Json;

namespace BusinessLogicLayer.Policies;

public class ProductsMicroservicePolicies : IProductsMicroservicePolicies
{
    private readonly ILogger<ProductsMicroservicePolicies> _logger;

    public ProductsMicroservicePolicies(ILogger<ProductsMicroservicePolicies> logger)
    {
        _logger = logger;
    }

    public IAsyncPolicy<HttpResponseMessage> GetBulkheadIsolationPolicy()
    {
        AsyncBulkheadPolicy<HttpResponseMessage> policy = Policy.BulkheadAsync<HttpResponseMessage>(
          maxParallelization: 2, //Allows up to 2 concurrent requests
          maxQueuingActions: 40, //Queue up to 40 additional requests
          onBulkheadRejectedAsync: (context) => {
              _logger.LogWarning("BulkheadIsolation triggered. Can't send any more requests since the queue is full");

              throw new BulkheadRejectedException("Bulkhead queue is full");
          }
          );

        return policy;
    }

    public IAsyncPolicy<HttpResponseMessage> GetFallbackPolicy()
    {
        AsyncFallbackPolicy<HttpResponseMessage> policy =
        Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .FallbackAsync( async(context) =>
            {
                _logger.LogWarning("Fallback executed for Products Microservice due to unsuccessful response");
                ProductDTO fallbackProduct = new(
                    ProductID: Guid.Empty,
                    ProductName: "Temporarily Unavailable",
                    Category: "Temporarily Unavailable",
                    UnitPrice: 0,
                    QuantityInStock: 0
                );
                var response = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                {
                    Content = new StringContent(JsonSerializer.Serialize(fallbackProduct),
                    Encoding.UTF8,
                    "application/json")
                };
                return response;
            });
        return policy;
    }

    public IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy()
    {
        var getBulkHeadIsolationPolicy = GetBulkheadIsolationPolicy();
        var getFallbackPolicy = GetFallbackPolicy();
        AsyncPolicyWrap<HttpResponseMessage> wrappedPolicy = Policy.WrapAsync(getBulkHeadIsolationPolicy, getFallbackPolicy);
        return wrappedPolicy;
    }
}
