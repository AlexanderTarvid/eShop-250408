namespace eShop.Ordering.API.Infrastructure.Services;

public class IdentityService(IHttpContextAccessor context) : IIdentityService
{
    public string GetUserIdentity()
        => context.HttpContext?.User.FindFirst("sub")?.Value ?? string.Empty;

    public string GetUserName()
        => context.HttpContext?.User.Identity?.Name ?? string.Empty;
}
