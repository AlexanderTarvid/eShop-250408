namespace eShop.Identity.API.Services
{
    public interface IRedirectUriWhitelistService
    {
        HashSet<string> GetWhitelistedUris();
    }
}