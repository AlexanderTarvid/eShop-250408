namespace eShop.Identity.API.Configuration;

public class RedirectSettings
{
    public const string SectionName = "Redirect";
    
    public List<string> WhitelistedUris { get; set; } = new List<string>();
} 