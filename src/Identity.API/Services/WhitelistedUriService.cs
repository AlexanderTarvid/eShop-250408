using eShop.Identity.API.Configuration;
using Microsoft.Extensions.Options;

namespace eShop.Identity.API.Services;

public class WhitelistedUriService
{
    public HashSet<string> WhitelistedUris { get; }

    public WhitelistedUriService(IOptions<RedirectOptions> options)
    {
        WhitelistedUris = new HashSet<string>(options.Value.WhitelistedUris, StringComparer.OrdinalIgnoreCase);
    }
}