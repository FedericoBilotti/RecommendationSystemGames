using Microsoft.EntityFrameworkCore;
using RM.Infrastructure.Database;
using RM.Presentation.StartUp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.AddDependencies();

var app = builder.Build();

// using (var scope = app.Services.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     context.Database.Migrate();
// }

app.UseOpenApi();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();