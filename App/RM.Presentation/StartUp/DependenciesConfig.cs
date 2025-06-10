using App;
using App.Interfaces;
using App.Interfaces.Authentication;
using App.Interfaces.Engine;
using App.Services.Authenticate;
using App.Services.Validators.Users;
using App.UseCases.Authentication;
using App.UseCases.Engine;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using RM.Domain.Entities;
using RM.Infrastructure.Data;
using RM.Infrastructure.Database;

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
        builder.Services.AddScoped<IValidationService, UserValidationService>();
        builder.Services.AddScoped<ITokenService, AuthTokenService>();
        builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        // Tokenization
        builder.Services.AddScoped<AuthTokenUseCase, AuthTokenUseCase>();
        builder.Services.AddScoped<AuthenticateUserUseCase, AuthenticateUserUseCase>();
    }
}