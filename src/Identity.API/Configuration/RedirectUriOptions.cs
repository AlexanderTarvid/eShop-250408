namespace eShop.Identity.API.Configuration
{
    public class RedirectUriOptions
    {
        public const string SectionName = "RedirectUriSettings";
        
        public List<string> WhitelistedUris { get; set; } = new();
    }
}