namespace eShop.Identity.API.Models;

public class RedirectSettings
{
    public const string SectionName = "RedirectSettings";
    
    public List<string> WhitelistedUris { get; set; } = new();
}
