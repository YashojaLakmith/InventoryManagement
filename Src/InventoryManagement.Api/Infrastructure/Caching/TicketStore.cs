using System.Security.Cryptography;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Hybrid;

namespace InventoryManagement.Api.Infrastructure.Caching;

public class TicketStore : ITicketStore
{
    private readonly HybridCache _hybridCache;
    private readonly ILogger<TicketStore> _logger;
    private readonly TicketSerializer _ticketSerializer;

    private static readonly HybridCacheEntryOptions NullEntryOptions = new() { Expiration = TimeSpan.FromSeconds(5) };
    private static readonly HybridCacheEntryOptions StandardExpiration = new() { Expiration = TimeSpan.FromMinutes(30) };

    public TicketStore(HybridCache hybridCache, ILogger<TicketStore> logger)
    {
        _hybridCache = hybridCache;
        _logger = logger;
        _ticketSerializer = TicketSerializer.Default;
    }

    public Task RemoveAsync(string key)
    {
        return _hybridCache.RemoveAsync(key).AsTask();
    }

    public Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        byte[] bytes = _ticketSerializer.Serialize(ticket);
        string hexData = Convert.ToHexString(bytes);
        return _hybridCache.SetAsync(key, hexData, StandardExpiration).AsTask();
    }

    public async Task<AuthenticationTicket?> RetrieveAsync(string key)
    {
        string? hexData = await _hybridCache.GetOrCreateAsync(
            key,
            async ct => await ValueTask.FromResult<string?>(null),
            NullEntryOptions);

        return hexData is null
            ? null
            : _ticketSerializer.Deserialize(Convert.FromHexString(hexData));
    }

    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        byte[] randomBytes = RandomNumberGenerator.GetBytes(256);
        string hexKey = Convert.ToHexString(randomBytes);
        byte[] bytes = _ticketSerializer.Serialize(ticket);
        string hexData = Convert.ToHexString(bytes);

        await _hybridCache.SetAsync(hexKey, hexData, StandardExpiration);

        return hexKey;
    }
}
