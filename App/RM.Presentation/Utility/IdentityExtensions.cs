using System.Security.Claims;

namespace RM.Presentation.Utility;

public static class IdentityExtensions
{
    public static Guid? GetUserId(this HttpContext context)
    {
        Claim? userId = context.User.Claims.SingleOrDefault(x => x.Type == "userid");
        
        if (Guid.TryParse(userId?.Value, out Guid userIdGuid))
            return userIdGuid;
        
        return null;
    }
}