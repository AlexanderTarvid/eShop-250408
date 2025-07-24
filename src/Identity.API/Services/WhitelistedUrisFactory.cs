using Microsoft.Extensions.Options;
using eShop.Identity.API.Models;

namespace eShop.Identity.API.Services;

public interface IWhitelistedUrisFactory
{
    HashSet<string> GetWhitelistedUris();
}

public class WhitelistedUrisFactory : IWhitelistedUrisFactory
{
    private readonly HashSet<string> _whitelistedUris;

    public WhitelistedUrisFactory(IOptions<RedirectSettings> redirectSettings)
    {
        _whitelistedUris = new HashSet<string>(
            redirectSettings.Value.WhitelistedUris,
            StringComparer.OrdinalIgnoreCase);
    }

    public HashSet<string> GetWhitelistedUris()
    {
        return _whitelistedUris;
    }
}
