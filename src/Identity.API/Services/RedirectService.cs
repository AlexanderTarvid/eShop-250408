using System.Web;
using Microsoft.AspNetCore.WebUtilities;

namespace eShop.Identity.API.Services
{
    public class RedirectService : IRedirectService
    {
        private readonly IRedirectUriWhitelistService _whitelistService;

        public RedirectService(IRedirectUriWhitelistService whitelistService)
        {
            _whitelistService = whitelistService;
        }

        public string ExtractRedirectUriFromReturnUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return string.Empty;

            try
            {
                // Parse the URL to extract query parameters safely
                var uri = new Uri(url, UriKind.RelativeOrAbsolute);
                
                string queryString;
                if (uri.IsAbsoluteUri)
                {
                    queryString = uri.Query;
                }
                else
                {
                    // Handle relative URLs by extracting query string portion
                    var queryIndex = url.IndexOf('?');
                    if (queryIndex == -1)
                        return string.Empty;
                    queryString = url.Substring(queryIndex);
                }

                // Parse query parameters using ASP.NET Core's safe query parser
                var queryParams = QueryHelpers.ParseQuery(queryString);
                
                if (!queryParams.TryGetValue("redirect_uri", out var redirectUriValues) || 
                    redirectUriValues.Count == 0)
                    return string.Empty;

                // Take only the first redirect_uri to prevent multiple encoding attacks
                var redirectUri = redirectUriValues.First();
                
                if (string.IsNullOrWhiteSpace(redirectUri))
                    return string.Empty;

                // URL decode the redirect URI safely (handles multiple encoding)
                var decodedUri = HttpUtility.UrlDecode(redirectUri);
                
                // Double decode to handle potential double encoding attacks
                var fullyDecodedUri = HttpUtility.UrlDecode(decodedUri);
                
                // Validate that the URI is properly formed
                if (!Uri.TryCreate(fullyDecodedUri, UriKind.Absolute, out var validatedUri))
                    return string.Empty;

                // Check against whitelist
                var whitelistedUris = _whitelistService.GetWhitelistedUris();
                var normalizedUri = validatedUri.ToString().TrimEnd('/');
                
                if (!whitelistedUris.Contains(normalizedUri))
                    return string.Empty;

                return normalizedUri;
            }
            catch
            {
                // Handle any parsing errors gracefully
                return string.Empty;
            }
        }
    }
}
