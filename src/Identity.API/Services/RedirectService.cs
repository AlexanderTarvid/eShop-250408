using Microsoft.AspNetCore.WebUtilities;
using System.Net;

namespace eShop.Identity.API.Services
{
    public class RedirectService(HashSet<string> whitelistedUris) : IRedirectService
    {

        public string ExtractRedirectUriFromReturnUrl(string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                    return string.Empty;

                // Parse the URL using native ASP.NET Core URL handling
                if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                    return string.Empty;

                // Extract query parameters safely
                var queryParams = QueryHelpers.ParseQuery(uri.Query);
                
                // Look for redirect_uri parameter
                if (!queryParams.TryGetValue("redirect_uri", out var redirectUriValues) || 
                    redirectUriValues.Count == 0)
                    return string.Empty;

                // Consider only the first redirect URI to prevent confusion attacks
                var redirectUriValue = redirectUriValues.First();
                if (string.IsNullOrWhiteSpace(redirectUriValue))
                    return string.Empty;

                // Decode the redirect URI properly to handle multiple encoding attacks
                var decodedRedirectUri = WebUtility.UrlDecode(redirectUriValue);
                if (string.IsNullOrWhiteSpace(decodedRedirectUri))
                    return string.Empty;

                // Validate that the decoded URI is well-formed
                if (!Uri.TryCreate(decodedRedirectUri, UriKind.Absolute, out var redirectUri))
                    return string.Empty;

                // Check if the redirect URI is in the whitelist
                var redirectUriString = redirectUri.ToString();
                if (!whitelistedUris.Contains(redirectUriString))
                    return string.Empty;

                return redirectUriString;
            }
            catch
            {
                // Handle any errors gracefully by returning empty string
                return string.Empty;
            }
        }
    }
}
