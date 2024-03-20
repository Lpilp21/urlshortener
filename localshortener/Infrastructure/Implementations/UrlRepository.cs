using Cassandra;
using localshortener.api.Exceptions;
using localshortener.api.Infrastructure.Contracts;
using System;

namespace localshortener.api.Infrastructure.Implementations;
public class UrlRepository : IUrlRepository
{
    private readonly IConfiguration _config;
    private readonly ILogger<UrlRepository> _logger;

    public UrlRepository(IConfiguration config, ILogger<UrlRepository> logger)
    {
        _config = config;
        _logger = logger;
    }
    public async Task<string> GetUrl(string url)
    {
        var session = Cluster.Builder()
                                .AddContactPoint(_config.GetValue<string>("Cassandra:Host"))
                                .WithPort(_config.GetValue<int>("Cassandra:Port"))
                                .Build()
                                .Connect(_config.GetValue<string>("Cassandra:Keyspace"));
        var preparedStatement = await session.PrepareAsync("SELECT url FROM urls where short_url = :ShortUrl");
        var boundStatement = preparedStatement.Bind(new {ShortUrl = url});
        var rs = await session.ExecuteAsync(boundStatement);

        foreach (var row in rs)
        {
            return  row.GetValue<string>("url");
        }
        throw new UrlNotFoundException();
    }

    public async Task<string> GenerateShorten(string longUrl, string? shortUrl)
    {
        var session = Cluster.Builder()
                        .AddContactPoint(_config.GetValue<string>("Cassandra:Host"))
                        .WithPort(_config.GetValue<int>("Cassandra:Port"))
                        .Build()
                        .Connect(_config.GetValue<string>("Cassandra:Keyspace"));
        if (shortUrl is null)
        {
            shortUrl = "auto-generated";
        }
        var preparedStatement = await session.PrepareAsync("insert into urls(short_url, url) VALUES (:shortUrl, :longUrl)");
        var boundStatement = preparedStatement.Bind(new { shortUrl = shortUrl, longUrl = longUrl });
        await session.ExecuteAsync(boundStatement);

        return shortUrl;
    }
}

