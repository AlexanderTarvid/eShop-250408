using Microsoft.AspNetCore.WebUtilities;

namespace eShop.Identity.API.Services
{
    public class RedirectService : IRedirectService
    {
        private readonly HashSet<string> _whitelistedUris;
        private readonly ILogger<RedirectService> _logger;

        public RedirectService(HashSet<string> whitelistedUris, ILogger<RedirectService> logger)
        {
            _whitelistedUris = whitelistedUris ?? throw new ArgumentNullException(nameof(whitelistedUris));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string ExtractRedirectUriFromReturnUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return "";
            }

            try
            {
                // Parse the URL to handle multiple encodings
                var decodedUrl = System.Net.WebUtility.UrlDecode(url);
                
                // Use ASP.NET Core's QueryHelpers to safely extract query parameters
                var queryIndex = decodedUrl.IndexOf('?');
                if (queryIndex == -1)
                {
                    return "";
                }

                var queryString = decodedUrl.Substring(queryIndex);
                var queryParams = QueryHelpers.ParseQuery(queryString);

                // Look for redirect_uri parameter
                if (!queryParams.TryGetValue("redirect_uri", out var redirectUriValues) || 
                    redirectUriValues.Count == 0)
                {
                    return "";
                }

                // Take only the first redirect URI to prevent multiple URI attacks
                var redirectUri = redirectUriValues[0];
                
                if (string.IsNullOrWhiteSpace(redirectUri))
                {
                    return "";
                }

                // Validate the redirect URI
                if (!IsValidRedirectUri(redirectUri))
                {
                    _logger.LogWarning("Invalid or non-whitelisted redirect URI attempted: {RedirectUri}", redirectUri);
                    return "";
                }

                return redirectUri;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting redirect URI from return URL: {Url}", url);
                return "";
            }
        }

        private bool IsValidRedirectUri(string redirectUri)
        {
            try
            {
                // Parse the URI to validate it's well-formed
                if (!Uri.TryCreate(redirectUri, UriKind.Absolute, out var parsedUri))
                {
                    return false;
                }

                // Normalize the URI for comparison
                var normalizedUri = parsedUri.GetLeftPart(UriPartial.Authority) + parsedUri.AbsolutePath.TrimEnd('/');

                // Check against whitelist
                return _whitelistedUris.Contains(normalizedUri);
            }
            catch
            {
                return false;
            }
        }
    }
}
