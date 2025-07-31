using Microsoft.AspNetCore.WebUtilities;

namespace eShop.Identity.API.Services;

public class RedirectService(HashSet<string> whitelistedUris) : IRedirectService
{
    public string ExtractRedirectUriFromReturnUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return string.Empty;
        }

        try
        {
            var query = QueryHelpers.ParseQuery(new Uri(url).Query);
            var redirectUri = query["redirect_uri"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(redirectUri))
            {
                return string.Empty;
            }

            var decodedRedirectUri = Uri.UnescapeDataString(redirectUri);

            if (!Uri.TryCreate(decodedRedirectUri, UriKind.Absolute, out _))
            {
                return string.Empty;
            }

            return whitelistedUris.Contains(decodedRedirectUri) ? decodedRedirectUri : string.Empty;
        }
        catch (UriFormatException)
        {
            return string.Empty;
        }
    }
}
