using System.Text;
using App.RM.Application.Interfaces;
using App.RM.Application.UseCases;
using App.RM.Domain.Entities;
using App.RM.Infrastructure.Data;
using App.RM.Infrastructure.Database;
using App.RM.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("UserDatabase"));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["AppSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = builder.Configuration["AppSettings:Audience"],
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!)),
                ValidateIssuerSigningKey = true
            };
        });

builder.Services.AddScoped<IAuthService, AuthenticateUserService>();
builder.Services.AddScoped<ITokenService, AuthTokenService>();
builder.Services.AddScoped<IUserRepository, DatabaseUserRepository>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<AuthTokenUseCase, AuthTokenUseCase>();
builder.Services.AddScoped<AuthenticateUserUseCase, AuthenticateUserUseCase>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    services.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();