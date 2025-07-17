using eShop.Identity.API.Configuration;
using Microsoft.Extensions.Options;

namespace eShop.Identity.API.Services
{
    public class RedirectUriWhitelistService(IOptions<RedirectUriOptions> options) : IRedirectUriWhitelistService
    {
        private readonly HashSet<string> _whitelistedUris = new(
            options.Value.WhitelistedUris ?? [],
            StringComparer.OrdinalIgnoreCase);

        public HashSet<string> GetWhitelistedUris() => _whitelistedUris;
    }
}