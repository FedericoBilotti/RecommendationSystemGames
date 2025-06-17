using App.Mappers;
using RM.Infrastructure.Database;
using RM.Presentation.StartUp;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.AddDependencies();
builder.AddJwtDependencies();

WebApplication app = builder.Build();

app.UseOpenApi();

// app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ValidationMappingMiddleware>();
app.MapControllers();

var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
await dbInitializer.InitializeDbAsync();

app.Run();

public partial class Program;