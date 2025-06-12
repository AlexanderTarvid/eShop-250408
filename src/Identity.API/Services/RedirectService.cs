using Microsoft.AspNetCore.WebUtilities;
using System.Web;

namespace eShop.Identity.API.Services
{
    public class RedirectService : IRedirectService
    {
        private readonly IRedirectUriWhitelistService _whitelistService;
        private readonly ILogger<RedirectService> _logger;

        public RedirectService(IRedirectUriWhitelistService whitelistService, ILogger<RedirectService> logger)
        {
            _whitelistService = whitelistService;
            _logger = logger;
        }

        public string ExtractRedirectUriFromReturnUrl(string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    _logger.LogWarning("Received null or empty URL");
                    return "";
                }

                // Parse the URL to extract query parameters safely
                if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri))
                {
                    _logger.LogWarning("Invalid URL format: {Url}", url);
                    return "";
                }

                // Extract query parameters
                var query = uri.Query;
                if (string.IsNullOrEmpty(query))
                {
                    _logger.LogWarning("No query parameters found in URL");
                    return "";
                }

                // Parse query parameters safely using ASP.NET Core utilities
                var queryDict = QueryHelpers.ParseQuery(query);
                
                // Look for redirect_uri parameter
                if (!queryDict.TryGetValue("redirect_uri", out var redirectUriValues) || redirectUriValues.Count == 0)
                {
                    _logger.LogWarning("No redirect_uri parameter found in URL");
                    return "";
                }

                // Get the first redirect URI value (handle multiple encoding attacks)
                var redirectUri = redirectUriValues.First();
                
                if (string.IsNullOrWhiteSpace(redirectUri))
                {
                    _logger.LogWarning("Empty redirect_uri parameter");
                    return "";
                }

                // Decode the URI properly to handle multiple encoding attacks
                var decodedUri = HttpUtility.UrlDecode(redirectUri);
                
                // Validate that the decoded URI is well-formed
                if (!Uri.TryCreate(decodedUri, UriKind.Absolute, out var parsedUri))
                {
                    _logger.LogWarning("Invalid redirect URI format: {RedirectUri}", decodedUri);
                    return "";
                }

                // Normalize the URI to prevent bypass attempts
                var normalizedUri = parsedUri.ToString();

                // Check if the URI is in the whitelist
                if (!_whitelistService.IsAllowed(normalizedUri))
                {
                    _logger.LogWarning("Redirect URI not whitelisted: {RedirectUri}", normalizedUri);
                    return "";
                }

                _logger.LogInformation("Valid redirect URI extracted: {RedirectUri}", normalizedUri);
                return normalizedUri;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting redirect URI from URL: {Url}", url);
                return "";
            }
        }
    }
}
