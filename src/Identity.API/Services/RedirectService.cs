namespace eShop.Identity.API.Services
{
    public class RedirectService : IRedirectService
    {
        private readonly HashSet<string> _whitelistedUris;
        private const int MaxDecodeRounds = 3;

        public RedirectService(HashSet<string> whitelistedUris)
        {
            _whitelistedUris = whitelistedUris;
        }

        public string ExtractRedirectUriFromReturnUrl(string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                    return string.Empty;

                // Parse query string (handle both full URLs and query fragments)
                string query = url;
                if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
                {
                    query = uri.Query;
                }
                else if (url.StartsWith("?"))
                {
                    query = url;
                }
                else if (url.Contains("?"))
                {
                    query = url.Substring(url.IndexOf('?'));
                }

                var parsed = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(query);
                if (!parsed.TryGetValue("returnUrl", out var returnUrlValues) && !parsed.TryGetValue("redirect_uri", out returnUrlValues))
                    return string.Empty;

                // Only consider the first return/redirect URI
                string redirectUri = returnUrlValues.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(redirectUri))
                    return string.Empty;

                // Defend against multiple encoding attacks
                for (int i = 0; i < MaxDecodeRounds; i++)
                {
                    var decoded = System.Net.WebUtility.UrlDecode(redirectUri);
                    if (decoded == redirectUri) break;
                    redirectUri = decoded;
                }

                // Validate absolute URI
                if (!Uri.TryCreate(redirectUri, UriKind.Absolute, out var parsedRedirectUri))
                    return string.Empty;

                // Compare with whitelist (exact match)
                if (_whitelistedUris.Contains(parsedRedirectUri.ToString()))
                    return parsedRedirectUri.ToString();
            }
            catch
            {
                // Graceful fallback
            }
            return string.Empty;
        }
    }
}
