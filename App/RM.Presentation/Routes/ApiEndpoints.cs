namespace RM.Presentation.Routes;

public static class ApiEndpoints
{
    private const string API_BASE = "api";

    public static class V1
    {
        private const string V1_BASE = $"{API_BASE}/v1";
        
        public static class Games
        {
            private const string BASE = $"{V1_BASE}/Games";
            
            public const string CREATE = $"{BASE}";
            public const string GET = $"{BASE}/{{idOrSlug}}";
            public const string GET_ALL = $"{BASE}";
            public const string UPDATE = $"{BASE}/{{id:Guid}}";
            public const string DELETE = $"{BASE}/{{id:Guid}}";
            
            public const string RATE = $"{BASE}/{{id:Guid}}/ratings";
            public const string DELETE_RATE = $"{BASE}/{{id:Guid}}/ratings";
        }

        public static class Ratings
        {
            private const string BASE = $"{V1_BASE}/ratings";
            
            public const string GET_USER_RATINGS = $"{BASE}/me";
        }
    }
}