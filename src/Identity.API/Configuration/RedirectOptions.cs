namespace eShop.Identity.API.Configuration;

public class RedirectOptions
{
    public const string SectionName = "RedirectOptions";
    public List<string> WhitelistedUris { get; set; } = new();
}