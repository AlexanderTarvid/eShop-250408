namespace eShop.Identity.API.Services;

public interface IRedirectUriWhitelistService
{
    bool IsAllowed(string uri);
} 