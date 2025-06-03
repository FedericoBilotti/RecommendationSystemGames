using System.Text;
using App;
using App.Interfaces.Authentication;
using App.Interfaces.Engine;
using App.RM.Application.Interfaces;
using App.Services;
using App.Services.Authenticate;
using App.Services.Engine;
using App.UseCases.Authentication;
using App.UseCases.Engine;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RM.Domain.Entities;
using RM.Infrastructure.Data;
using RM.Infrastructure.Database;
using RM.Infrastructure.StartUp;

namespace RM.Presentation.StartUp;

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