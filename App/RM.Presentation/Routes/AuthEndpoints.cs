namespace RM.Presentation.Routes;

public static class AuthEndpoints
{
    private const string AUTH_BASE = "auth";
    
    public static class Auth 
    {
        public const string REGISTER = $"{AUTH_BASE}/register";
        public const string LOGIN = $"{AUTH_BASE}/login";
        public const string REFRESH_TOKEN = $"{AUTH_BASE}/refresh-token";
    }
}