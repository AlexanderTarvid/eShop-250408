using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Web;
using System.Text.RegularExpressions;

namespace eShop.Identity.API.Services
{
    public class RedirectUriWhitelistOptions
    {
        public List<string> RedirectUriWhitelist { get; set; } = new();
    }

    public class RedirectService : IRedirectService
    {
        private readonly HashSet<string> _whitelist;

        public RedirectService(HashSet<string> whitelist)
        {
            _whitelist = whitelist;
        }

        public string ExtractRedirectUriFromReturnUrl(string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                    return string.Empty;

                // Multiple encoding attack mitigation: decode repeatedly up to a safe limit
                string decodedUrl = url;
                for (int i = 0; i < 3; i++)
                {
                    var temp = System.Net.WebUtility.UrlDecode(decodedUrl);
                    if (temp == decodedUrl) break;
                    decodedUrl = temp;
                }

                // Use regex to find the first redirect_uri parameter
                var match = Regex.Match(decodedUrl, @"[?&]redirect_uri=([^&#]*)", RegexOptions.IgnoreCase);
                if (!match.Success)
                    return string.Empty;

                var redirectUriRaw = match.Groups[1].Value;
                // Remove any trailing parameters
                var ampIndex = redirectUriRaw.IndexOf('&');
                if (ampIndex > -1)
                    redirectUriRaw = redirectUriRaw.Substring(0, ampIndex);

                // Decode the redirect URI value (again, up to a safe limit)
                string redirectUri = redirectUriRaw;
                for (int i = 0; i < 3; i++)
                {
                    var temp = System.Net.WebUtility.UrlDecode(redirectUri);
                    if (temp == redirectUri) break;
                    redirectUri = temp;
                }

                // Validate using native .NET URI parsing
                if (!Uri.TryCreate(redirectUri, UriKind.Absolute, out var parsedUri))
                    return string.Empty;

                // Check whitelist
                if (_whitelist.Contains(parsedUri.ToString()))
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
