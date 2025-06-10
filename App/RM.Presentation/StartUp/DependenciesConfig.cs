using System.Text;
using App;
using App.Dtos.Authentication.Request;
using App.Interfaces;
using App.Interfaces.Authentication;
using App.Interfaces.Engine;
using App.Services.Authenticate;
using App.Services.Validators.Users;
using App.UseCases.Authentication;
using App.UseCases.Engine;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RM.Infrastructure.Data;
using RM.Infrastructure.Database;
using RM.Presentation.Auth;

namespace RM.Presentation.StartUp;

public static class DependenciesConfig
{
    public static void AddDependencies(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApiServices();
        
        // Database
        string connectionString = builder.Configuration.GetConnectionString("UserDatabase")!;
        builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>(_ => new DbConnectionFactory(connectionString));
        builder.Services.AddSingleton<DbInitializer>();
        
        // Game
        builder.Services.AddScoped<IGameUseCase, GameUseCase>();
        builder.Services.AddScoped<IRatingUseCase, RatingUseCase>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IGamesRepository, GamesRepository>();
        builder.Services.AddScoped<IRatingRepository, RatingRepository>();
        builder.Services.AddValidatorsFromAssemblyContaining<IApplicationMarker>();
        
        // Authentication 
        builder.Services.AddScoped<IPasswordHasher<UserRegisterRequestDto>, PasswordHasher<UserRegisterRequestDto>>();
        builder.Services.AddScoped<IValidationService, UserValidationService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<AuthTokenUseCase, AuthTokenUseCase>();
        builder.Services.AddScoped<IAuthenticateUserUseCase, AuthenticateUserUseCase>();
    }

    public static void AddJwtDependencies(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthConstants.ADMIN_ROLE, policy => policy.RequireClaim(AuthConstants.ADMIN_CLAIM, "true"));
            options.AddPolicy(AuthConstants.TRUSTED_ROLE, 
                    policy => policy.RequireAssertion(x => 
                            x.User.HasClaim(m => m is { Type: AuthConstants.ADMIN_CLAIM, Value: "true" }) ||
                            x.User.HasClaim(m => m is { Type: AuthConstants.TRUSTED_CLAIM, Value: "true" })));
        });

        builder.Services.AddAuthentication(authOptions =>
        {
            authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            authOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
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
    }
}