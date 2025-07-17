using Microsoft.AspNetCore.WebUtilities;

namespace eShop.Identity.API.Services
{
    public class RedirectService(IRedirectUriWhitelistService whitelistService) : IRedirectService
    {
        public string ExtractRedirectUriFromReturnUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return string.Empty;

            try
            {
                // Parse the URL using native ASP.NET Core QueryHelpers
                var queryParams = QueryHelpers.ParseQuery(url);
                
                if (!queryParams.TryGetValue("redirect_uri", out var redirectUriValues) || 
                    redirectUriValues.Count == 0)
                    return string.Empty;

                // Take only the first redirect_uri to prevent multiple parameter attacks
                var redirectUri = redirectUriValues.First();
                
                if (string.IsNullOrWhiteSpace(redirectUri))
                    return string.Empty;

                // URL decode using native ASP.NET Core WebUtilities (handles multiple encoding)
                var decodedUri = System.Net.WebUtility.UrlDecode(redirectUri);
                
                // Double decode to handle potential double encoding attacks
                var fullyDecodedUri = System.Net.WebUtility.UrlDecode(decodedUri);
                
                // Validate that the URI is properly formed using native Uri class
                if (!Uri.TryCreate(fullyDecodedUri, UriKind.Absolute, out var validatedUri))
                    return string.Empty;

                // Check against whitelist
                var whitelistedUris = whitelistService.GetWhitelistedUris();
                var uriString = validatedUri.ToString();
                
                if (!whitelistedUris.Contains(uriString))
                    return string.Empty;

                return uriString;
            }
            catch
            {
                // Handle any parsing errors gracefully
                return string.Empty;
            }
        }
    }
}
