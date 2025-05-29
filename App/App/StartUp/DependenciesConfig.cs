using System.Text;
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
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace App.StartUp;

public static class DependenciesConfig
{
    public static void AddDependencies(this WebApplicationBuilder builder)
    {
        Env.Load();

        builder.Services.AddOpenApiServices();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
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

        // Database
        builder.Services.AddScoped<IUserRepository, DatabaseUserRepository>();
        builder.Services.AddDbContext<AppDbContext>(options => { options.UseSqlServer(builder.Configuration.GetConnectionString("UserDatabase")); });

        // Authentication 
        builder.Services.AddScoped<IAuthService, AuthenticateUserService>();
        builder.Services.AddScoped<ITokenService, AuthTokenService>();
        builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        // Tokenization
        builder.Services.AddScoped<AuthTokenUseCase, AuthTokenUseCase>();
        builder.Services.AddScoped<AuthenticateUserUseCase, AuthenticateUserUseCase>();

        // Engine
        builder.Services.Configure<RawgApiSettings>(options => options.RawgApikey = Environment.GetEnvironmentVariable("RAWG_APIKEY"));
        builder.Services.AddHttpClient<IEngine, EngineService>();
        builder.Services.AddScoped<IDeserializer, JsonDeserializerService>();
        builder.Services.AddScoped<IEngine, EngineService>();
        builder.Services.AddScoped<IEngineUseCase, EngineUseCase>();
    }
}