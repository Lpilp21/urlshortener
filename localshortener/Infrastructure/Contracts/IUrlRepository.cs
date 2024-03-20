namespace localshortener.api.Infrastructure.Contracts;
public interface IUrlRepository
{
    Task<string> GetUrl(string shortUrl);
    Task<string> GenerateShorten(string longUrl, string? shortUrl);
}

