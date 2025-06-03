namespace RM.Presentation.Routes;

public static class ApiEndpoints
{
    private const string API_BASE = "api";

    public static class V1
    {
        private const string V1_BASE = $"{API_BASE}/v1";
        
        public static class Games
        {
            public const string CREATE = $"{V1_BASE}/Games/Create";
            
            public const string FILTER = $"{V1_BASE}/Games/Filter";
            public const string FILTER_GENRES = $"{V1_BASE}/Games/FilterGenres";
            public const string FILTER_DEVELOPERS = $"{V1_BASE}/Games/FilterDevelopers";
        }    
    }
}