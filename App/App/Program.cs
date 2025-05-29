using System.Text;
using App;
using App.RM.Application.Interfaces;
using App.RM.Application.Interfaces.Authentication;
using App.RM.Application.Interfaces.Engine;
using App.RM.Application.UseCases.Authentication;
using App.RM.Application.UseCases.Engine;
using App.RM.Domain.Entities;
using App.RM.Infrastructure.Data;
using App.RM.Infrastructure.Database;
using App.RM.Infrastructure.Services;
using App.RM.Infrastructure.Services.Authenticate;
using App.RM.Infrastructure.Services.Engine;
using App.StartUp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.AddDependencies();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    services.Database.Migrate();
}

app.UseOpenApi();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();