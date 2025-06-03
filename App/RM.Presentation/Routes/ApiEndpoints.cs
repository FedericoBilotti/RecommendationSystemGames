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
            public const string GET = $"{BASE}/{{id:idOrSlug}}";
            public const string GET_ALL = $"{BASE}";
            public const string UPDATE = $"{BASE}/{{id:guid}}";
            public const string DELETE = $"{BASE}/{{id:guid}}";
        }    
    }
}