namespace eShop.Identity.API.Services
{
    public class RedirectUriWhitelistOptions
    {
        public List<string> RedirectUriWhitelist { get; set; } = new();
    }
}
