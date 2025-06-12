using Microsoft.AspNetCore.WebUtilities;
using System.Web;

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
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    return "";
                }

                // Parse the URL to extract query parameters safely
                if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri))
                {
                    return "";
                }

                // Extract query parameters
                var query = uri.Query;
                if (string.IsNullOrEmpty(query))
                {
                    return "";
                }

                // Parse query parameters safely using ASP.NET Core utilities
                var queryDict = QueryHelpers.ParseQuery(query);
                
                // Look for redirect_uri parameter
                if (!queryDict.TryGetValue("redirect_uri", out var redirectUriValues) || redirectUriValues.Count == 0)
                {
                    return "";
                }

                // Get the first redirect URI value (handle multiple encoding attacks)
                var redirectUri = redirectUriValues.First();
                
                if (string.IsNullOrWhiteSpace(redirectUri))
                {
                    return "";
                }

                // Decode the URI properly to handle multiple encoding attacks
                var decodedUri = HttpUtility.UrlDecode(redirectUri);
                
                // Validate that the decoded URI is well-formed
                if (!Uri.TryCreate(decodedUri, UriKind.Absolute, out var parsedUri))
                {
                    return "";
                }

                // Normalize the URI to prevent bypass attempts
                var normalizedUri = parsedUri.ToString();

                // Check if the URI is in the whitelist
                if (!_whitelistService.IsAllowed(normalizedUri))
                {
                    return "";
                }

                return normalizedUri;
            }
            catch
            {
                return "";
            }
        }
    }
}
