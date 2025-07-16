using eShop.Identity.API.Configuration;
using Microsoft.Extensions.Options;

namespace eShop.Identity.API.Services
{
    public class RedirectUriWhitelistService : IRedirectUriWhitelistService
    {
        private readonly HashSet<string> _whitelistedUris;

        public RedirectUriWhitelistService(IOptions<RedirectUriOptions> options)
        {
            _whitelistedUris = new HashSet<string>(
                options.Value.WhitelistedUris ?? new List<string>(),
                StringComparer.OrdinalIgnoreCase);
        }

        public HashSet<string> GetWhitelistedUris() => _whitelistedUris;
    }
}