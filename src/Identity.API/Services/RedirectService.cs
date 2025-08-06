using Microsoft.AspNetCore.WebUtilities;

namespace eShop.Identity.API.Services;

public class RedirectService(HashSet<string> whitelistedUris) : IRedirectService
{
    public string ExtractRedirectUriFromReturnUrl(string url)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(url))
                return string.Empty;

            var uri = new Uri(url, UriKind.RelativeOrAbsolute);
            var query = QueryHelpers.ParseQuery(uri.Query);
            
            if (!query.TryGetValue("redirect_uri", out var redirectUris) || redirectUris.Count == 0)
                return string.Empty;

            var redirectUri = redirectUris[0];
            if (string.IsNullOrWhiteSpace(redirectUri))
                return string.Empty;

            if (!Uri.TryCreate(redirectUri, UriKind.Absolute, out var validatedUri))
                return string.Empty;

            return whitelistedUris.Contains(validatedUri.ToString()) ? validatedUri.ToString() : string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
}
