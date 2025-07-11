namespace Identity.API.Models
{
    public class RedirectUriWhitelistOptions
    {
        public List<string> RedirectUriWhitelist { get; set; } = new();
    }
}
