using eShop.Identity.API.Configuration;
using Microsoft.Extensions.Options;

namespace eShop.Identity.API.Services;

public class RedirectUriWhitelistFactory
{
    public static HashSet<string> Create(IServiceProvider serviceProvider)
    {
        var options = serviceProvider.GetRequiredService<IOptions<RedirectSettings>>();
        var whitelist = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        
        foreach (var uri in options.Value.WhitelistedUris)
        {
            if (Uri.TryCreate(uri, UriKind.Absolute, out var parsedUri))
            {
                // Normalize the URI by using the absolute URI string
                whitelist.Add(parsedUri.GetLeftPart(UriPartial.Authority) + parsedUri.AbsolutePath.TrimEnd('/'));
            }
        }
        
        return whitelist;
    }
} 