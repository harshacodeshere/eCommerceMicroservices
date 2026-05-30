using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;
using System.ComponentModel;

namespace BusinessLogicLayer.Policies;

public class PollyPolicies : IPollyPolicies
{
    private readonly ILogger<UsersMicroservicePolicies> _logger;
    public PollyPolicies(ILogger<UsersMicroservicePolicies> logger)
    {
        _logger = logger;
    }

    public IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retries)
    {
        AsyncRetryPolicy<HttpResponseMessage> policy =
        Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(
                retryCount: retries,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    _logger.LogInformation($"Retry {retryAttempt} after {timespan.TotalSeconds} seconds due to " +
                        $"unsuccessful response: {outcome.Result.StatusCode}");
                }
            );
        return policy;
    }

    public IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(int breakingLimit, TimeSpan duration)
    {
        AsyncCircuitBreakerPolicy<HttpResponseMessage> policy =
        Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: breakingLimit,
                durationOfBreak: duration,
                onBreak: (outcome, timespan) =>
                {
                    _logger.LogInformation($"Circuit breaker opened for {timespan.TotalMinutes} due to consecutive failures");
                },
                onReset: () =>
                {
                    _logger.LogInformation("Circuit breaker reset");
                }
            );
        return policy;
    }

    public IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(TimeSpan timeSpan)
    {
        AsyncTimeoutPolicy<HttpResponseMessage> policy = Policy.TimeoutAsync<HttpResponseMessage>(timeSpan);

        return policy;
    }
}
