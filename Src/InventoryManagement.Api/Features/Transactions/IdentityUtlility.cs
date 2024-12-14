using System.Security.Claims;

namespace InventoryManagement.Api.Features.Transactions;

public static class IdentityUtlility
{
    public static string? ExtractEmailFromClaims(ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Email)?.Value;
    }
}
