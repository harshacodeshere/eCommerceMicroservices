using Polly;

namespace BusinessLogicLayer.Policies;

public interface IPollyPolicies
{
    IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retries);
    IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(int breakingLimit, TimeSpan duration);
    IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(TimeSpan timeSpan);
}
