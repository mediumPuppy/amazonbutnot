namespace amazonbutnot.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;

public class RateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;

    public RateLimitMiddleware(RequestDelegate next, IMemoryCache cache)
    {
        _next = next;
        _cache = cache;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientIdentifier = context.Connection.RemoteIpAddress.ToString();
        var currentTime = DateTimeOffset.Now;
        var windowStart = new DateTimeOffset(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, 0, 0, currentTime.Offset); // Fixed hourly window

        var cacheKey = $"{clientIdentifier}-{windowStart}";

        if (!_cache.TryGetValue(cacheKey, out int requestCount))
        {
            requestCount = 0;
        }

        if (requestCount >= 100000) // Example: Allow up to 200 requests per hour
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
            return;
        }

        requestCount++;
        var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(windowStart.AddHours(1));
        _cache.Set(cacheKey, requestCount, cacheEntryOptions);

        await _next(context);
    }
}

