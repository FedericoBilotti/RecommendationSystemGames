namespace RM.Presentation.StartUp;

public static class OpenApiConfig
{
    public static void AddOpenApiServices(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }
    
    public static void UseOpenApi(this WebApplication app)
    {
        // I can put this and then in the docker-compose put Development
        // if (!app.Environment.IsDevelopment()) return;
        
        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Recommendation Games API V1");
            c.RoutePrefix = "swagger";
        });
    }
}