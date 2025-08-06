using Microsoft.AspNetCore.WebUtilities;

namespace eShop.Identity.API.Services;

public class RedirectService : IRedirectService
{
    private readonly WhitelistedUriService _whitelistedUriService;

    public RedirectService(WhitelistedUriService whitelistedUriService)
    {
        _whitelistedUriService = whitelistedUriService;
    }

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

            // Handle multiple encoding by repeatedly decoding until no change
            string decoded = redirectUri;
            string previousDecoded;
            do
            {
                previousDecoded = decoded;
                decoded = Uri.UnescapeDataString(decoded);
            } while (decoded != previousDecoded);

            if (!Uri.TryCreate(decoded, UriKind.Absolute, out var validatedUri))
                return string.Empty;

            var normalizedUri = validatedUri.ToString();
            return _whitelistedUriService.WhitelistedUris.Contains(normalizedUri) ? normalizedUri : string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
}
