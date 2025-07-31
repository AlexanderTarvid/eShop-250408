using System.Web;

namespace eShop.Identity.API.Services;

public class RedirectService(HashSet<string> whitelistedUris) : IRedirectService
{
    public string ExtractRedirectUriFromReturnUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return string.Empty;
        }

        string redirectUri = string.Empty;
        try
        {
            var decodedUrl = HttpUtility.UrlDecode(url);
            var uri = new Uri(decodedUrl);
            var query = HttpUtility.ParseQueryString(uri.Query);
            redirectUri = query["redirect_uri"];

            if (string.IsNullOrWhiteSpace(redirectUri) || !whitelistedUris.Contains(redirectUri))
            {
                return string.Empty;
            }
        }
        catch (UriFormatException)
        {
            // Handle malformed URLs gracefully
            return string.Empty;
        }

        return redirectUri;
    }
}
