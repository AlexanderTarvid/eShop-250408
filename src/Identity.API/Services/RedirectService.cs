using Microsoft.AspNetCore.WebUtilities;

namespace eShop.Identity.API.Services
{
    public class RedirectService(HashSet<string> whitelist) : IRedirectService
    {
        public string ExtractRedirectUriFromReturnUrl(string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                    return string.Empty;

                // Use ASP.NET Core's QueryHelpers to parse the query string
                var uri = new Uri(url, UriKind.RelativeOrAbsolute);
                string query = uri.IsAbsoluteUri ? uri.Query : url;
                if (!query.Contains("redirect_uri"))
                {
                    // Try to extract query from a relative URL
                    var idx = url.IndexOf('?');
                    if (idx >= 0)
                        query = url.Substring(idx);
                }
                var queryDict = QueryHelpers.ParseQuery(query);
                if (!queryDict.TryGetValue("redirect_uri", out var redirectUris) || redirectUris.Count == 0)
                    return string.Empty;

                // Only consider the first redirect_uri
                string redirectUri = redirectUris[0];

                if (!Uri.TryCreate(redirectUri, UriKind.Absolute, out var parsedUri))
                    return string.Empty;

                if (whitelist.Contains(parsedUri.ToString()))
                    return parsedUri.ToString();
            }
            catch
            {
                // Graceful error handling
            }
            return string.Empty;
        }
    }
}
