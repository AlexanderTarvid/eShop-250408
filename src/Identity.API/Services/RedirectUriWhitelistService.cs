using Microsoft.Extensions.Options;
using eShop.Identity.API.Models;

namespace eShop.Identity.API.Services;

public class RedirectUriWhitelistService : IRedirectUriWhitelistService
{
    private readonly HashSet<string> _allowedUris;

    public RedirectUriWhitelistService(IOptions<RedirectUriWhitelistOptions> options)
    {
        _allowedUris = new HashSet<string>(options.Value.AllowedUris, StringComparer.OrdinalIgnoreCase);
    }

    public bool IsAllowed(string uri)
    {
        if (string.IsNullOrWhiteSpace(uri))
            return false;

        return _allowedUris.Contains(uri);
    }
} 