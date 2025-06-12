using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Collections.Frozen;

namespace eShop.Identity.API.Services
{
    public class RedirectService : IRedirectService
    {
        private readonly FrozenSet<string> _whitelistedUris;

        public RedirectService(IOptions<RedirectOptions> options)
        {
            _whitelistedUris = options.Value.WhitelistedUris?.ToFrozenSet(StringComparer.OrdinalIgnoreCase) ?? FrozenSet<string>.Empty;
        }

        public string ExtractRedirectUriFromReturnUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return string.Empty;
            }

            try
            {
                // Perform multiple rounds of URL decoding to handle multiple encoding attacks
                var decodedUrl = url;
                var previousUrl = string.Empty;
                var maxDecodingAttempts = 5; // Prevent infinite loops
                var attempts = 0;

                while (decodedUrl != previousUrl && attempts < maxDecodingAttempts)
                {
                    previousUrl = decodedUrl;
                    decodedUrl = Uri.UnescapeDataString(decodedUrl);
                    attempts++;
                }

                // Parse the URL to extract query parameters safely
                if (!Uri.TryCreate(decodedUrl, UriKind.RelativeOrAbsolute, out var uri))
                {
                    return string.Empty;
                }

                var queryParams = new Dictionary<string, string>();
                
                // Handle both absolute and relative URLs
                if (uri.IsAbsoluteUri && !string.IsNullOrEmpty(uri.Query))
                {
                    queryParams = QueryHelpers.ParseQuery(uri.Query)
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString(), StringComparer.OrdinalIgnoreCase);
                }
                else
                {
                    // For relative URLs, try to parse the entire string as query parameters
                    var queryStartIndex = decodedUrl.IndexOf('?');
                    if (queryStartIndex >= 0)
                    {
                        var queryString = decodedUrl.Substring(queryStartIndex);
                        queryParams = QueryHelpers.ParseQuery(queryString)
                            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString(), StringComparer.OrdinalIgnoreCase);
                    }
                    else
                    {
                        // If no query separator, try parsing the entire string as key-value pairs
                        var pairs = decodedUrl.Split('&', StringSplitOptions.RemoveEmptyEntries);
                        foreach (var pair in pairs)
                        {
                            var kvp = pair.Split('=', 2);
                            if (kvp.Length == 2)
                            {
                                queryParams[kvp[0]] = kvp[1];
                            }
                        }
                    }
                }

                // Look for redirect_uri parameter
                if (!queryParams.TryGetValue("redirect_uri", out var redirectUri) || string.IsNullOrWhiteSpace(redirectUri))
                {
                    return string.Empty;
                }

                // Handle multiple redirect URIs - take only the first one
                var redirectUris = redirectUri.Split(',', StringSplitOptions.RemoveEmptyEntries);
                var firstRedirectUri = redirectUris[0].Trim();

                // Validate the redirect URI format
                if (!Uri.TryCreate(firstRedirectUri, UriKind.Absolute, out var redirectUriObj))
                {
                    return string.Empty;
                }

                // Normalize the URI (remove fragments, normalize case, etc.)
                var normalizedUri = new UriBuilder(redirectUriObj)
                {
                    Fragment = string.Empty // Remove fragment for security
                }.Uri.ToString();

                // Check against whitelist
                if (!_whitelistedUris.Contains(normalizedUri))
                {
                    return string.Empty;
                }

                return normalizedUri;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }

    public class RedirectOptions
    {
        public const string SectionName = "Redirect";
        public List<string> WhitelistedUris { get; set; } = new();
    }
}
