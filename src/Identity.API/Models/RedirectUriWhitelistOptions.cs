namespace eShop.Identity.API.Models;

public class RedirectUriWhitelistOptions
{
    public const string SectionName = "RedirectUriWhitelist";
    
    public List<string> AllowedUris { get; set; } = new();
} 