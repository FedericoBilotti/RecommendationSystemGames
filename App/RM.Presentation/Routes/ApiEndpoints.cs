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
            public const string GET_ALL = $"{BASE}";
            public const string GET = $"{BASE}/{{id:guid}}";
            
            // public const string FILTER = $"{V1_BASE}/Games/Filter";
            // public const string FILTER_GENRES = $"{V1_BASE}/Games/FilterGenres";
            // public const string FILTER_DEVELOPERS = $"{V1_BASE}/Games/FilterDevelopers";
        }    
    }
}