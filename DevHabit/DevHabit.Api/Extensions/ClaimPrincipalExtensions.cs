using System.Security.Claims;

namespace DevHabit.Api.Extensions;

public static class ClaimPrincipalExtensions
{
    public static string? GetIdentityId(this ClaimsPrincipal? principal)
    {
        string? identityId = principal?.FindFirstValue(ClaimTypes.NameIdentifier);

        return identityId;
    }
}
