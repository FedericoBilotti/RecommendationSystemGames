using System.Security.Claims;

namespace RM.Presentation.Auth;

public static class IdentityExtensions
{
    public static Guid? GetUserId(this HttpContext context)
    {
        Claim? userId = context.User.Claims.SingleOrDefault(x => x.Type == "userId");
        
        if (Guid.TryParse(userId?.Value, out Guid userIdGuid))
            return userIdGuid;
        
        return null;
    }
}