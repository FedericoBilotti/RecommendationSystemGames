namespace RM.Presentation.Routes;

public static class AuthEndpoints
{
    private const string AUTH_BASE = "auth";
    
    public const string AUTHORIZED = $"{AUTH_BASE}/authorized";
    public const string ADMIN = $"{AUTH_BASE}/admin-only";
    public const string TRUSTED_USER = $"{AUTH_BASE}/trusted_user";
    
    public static class Auth
    {
        public const string REGISTER = $"{AUTH_BASE}/register";
        public const string LOGIN = $"{AUTH_BASE}/login";
        public const string REFRESH_TOKEN = $"{AUTH_BASE}/refresh-token";
        public const string REFRESH_TOKEN_ID = $"{AUTH_BASE}/refresh-token/{{id:guid}}";
    }
}