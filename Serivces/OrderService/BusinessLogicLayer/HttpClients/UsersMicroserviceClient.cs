using BusinessLogicLayer.DTO;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;
using System.Net.Http.Json;
using System.Text.Json;

namespace BusinessLogicLayer.HttpClients;

public class UsersMicroserviceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UsersMicroserviceClient> _logger;
    private readonly IDistributedCache _distributedCache;

    public UsersMicroserviceClient(HttpClient httpClient, ILogger<UsersMicroserviceClient> logger, IDistributedCache distributedCache)
    {
        _httpClient = httpClient;
        _logger = logger;
        _distributedCache = distributedCache;
    }

    public async Task<UserDTO?> GetUserByUserID(Guid userID)
    {
        try
        {
            string cacheKey = $"user:{userID}";
            string? cachedUser = await _distributedCache.GetStringAsync(cacheKey);

            if (cachedUser != null)
            {
                _logger.LogInformation("Product retrieved from cache.");
                return JsonSerializer.Deserialize<UserDTO>(cachedUser);
            }

            HttpResponseMessage response = await _httpClient.GetAsync($"/gateway/Users/{userID}");

            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<UserDTO>() ?? throw new HttpRequestException("Failed to deserialize user data", null, System.Net.HttpStatusCode.InternalServerError);

                string userJson = JsonSerializer.Serialize(user);
                DistributedCacheEntryOptions cacheOptions = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2));
                string cacheKeyToWrite = $"user:{userID}";
                await _distributedCache.SetStringAsync(cacheKeyToWrite, userJson, cacheOptions);
                return user;
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null; // User not found
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    throw new HttpRequestException("Bad request", null, System.Net.HttpStatusCode.BadRequest);
                }
                else
                {
                    //throw new HttpRequestException($"Http request failed with status code {response.StatusCode}");
                    return new UserDTO(
                        UserID: Guid.Empty,
                        Email: "Temporarily Unavailable",
                        PersonName: "Temporarily Unavailable",
                        Gender: "Temporarily Unavailable"
                    );
                }
            }
        }
        catch (BrokenCircuitException ex)
        {
            _logger.LogInformation("Circuit is opened");
            return new UserDTO(
                UserID: Guid.Empty,
                Email: "Temporarily Unavailable",
                PersonName: "Temporarily Unavailable",
                Gender: "Temporarily Unavailable"
            );
        }
    }
}
