using localshortener.api.Dtos;
using localshortener.api.Infrastructure.Contracts;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Text;

namespace localshortener.api.Presentation;
public static class ShortenModule
{
    public static void AddShortenerEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/{shortUrl}", async (IUrlRepository repo, IDistributedCache cache, string shortUrl) =>
        {
            var url = await cache.GetAsync(shortUrl);

            if (url == null)
            {
                var apiUrl = await repo.GetUrl(shortUrl);
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1));
                var encodedUrl = Encoding.UTF8.GetBytes(apiUrl);
                await cache.SetAsync(shortUrl, encodedUrl, options);

                return Results.Redirect(apiUrl);
            }

            return Results.Redirect(Encoding.UTF8.GetString(url));
        });
        app.MapPost("/shorten", async (IUrlRepository repo, UrlDto url) =>
        {
            var shortUrl = await repo.GenerateShorten(url.Url, url.CostomUrl);
            return shortUrl;
        });
    }
}

