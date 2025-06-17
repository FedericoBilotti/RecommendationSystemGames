using System.Text;
using App;
using App.Auth;
using App.Dtos.Authentication.Request;
using App.Interfaces;
using App.Interfaces.Authentication;
using App.Interfaces.Engine;
using App.Services;
using App.Services.Authenticate;
using App.Services.Validators.Users;
using App.UseCases.Authentication;
using App.UseCases.Engine;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RM.Domain.Entities;
using RM.Infrastructure.Data;
using RM.Infrastructure.Database;
using RM.Presentation.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RM.Presentation.StartUp;

public static class DependenciesConfig
{
    public static void AddDependencies(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApiServices();
        
        // Swagger
        builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        // Database
        string connectionString = Environment.GetEnvironmentVariable("USER_DATABASE")!;
        builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>(_ => new DbConnectionFactory(connectionString));
        builder.Services.AddSingleton<DbInitializer>();

        // Repositories
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IGamesRepository, GamesRepository>();
        builder.Services.AddScoped<IRatingRepository, RatingRepository>();
        builder.Services.AddValidatorsFromAssemblyContaining<IApplicationMarker>();

        // Game
        builder.Services.AddScoped<IGameUseCase, GameUseCase>();
        builder.Services.AddScoped<IRatingUseCase, RatingUseCase>();

        // Hasher                                                                
        builder.Services.AddScoped<IPasswordHasher<UserRegisterRequestDto>, PasswordHasher<UserRegisterRequestDto>>();
        builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        builder.Services.AddScoped<IHasherService, HasherService>();

        // Authentication 
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IUserValidationService, UserValidationService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<AuthTokenUseCase, AuthTokenUseCase>();
        builder.Services.AddScoped<IAuthenticateUserUseCase, AuthenticateUserUseCase>();
    }

    public static void AddJwtDependencies(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthConstants.ADMIN_ROLE, 
                    policy => policy.RequireClaim(AuthConstants.ADMIN_CLAIM, "true"));
            options.AddPolicy(AuthConstants.TRUSTED_ROLE,
                    policy => policy.RequireAssertion(x =>
                            x.User.HasClaim(m => m is { Type: AuthConstants.ADMIN_CLAIM, Value: "true" }) || 
                            x.User.HasClaim(m => m is { Type: AuthConstants.TRUSTED_CLAIM, Value: "true" })));
        });

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Environment.GetEnvironmentVariable("ISSUER"),
                ValidateAudience = true,
                ValidAudience = Environment.GetEnvironmentVariable("AUDIENCE"),
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("TOKEN_KEY")!)),
                ValidateIssuerSigningKey = true
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.Token = context.Request.Cookies[TokenConstants.ACCESS_TOKEN];
                    return Task.CompletedTask;
                }
            };
        });
    }
}