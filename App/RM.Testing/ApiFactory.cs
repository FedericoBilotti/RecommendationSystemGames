using System;
using System.Threading;
using System.Threading.Tasks;
using App;
using App.Dtos.Authentication.Request;
using App.Interfaces;
using App.Interfaces.Authentication;
using App.Interfaces.Engine;
using App.Services;
using App.Services.Authenticate;
using App.Services.Validators.Users;
using App.UseCases.Authentication;
using App.UseCases.Engine;
using Dapper;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RM.Domain.Entities;
using RM.Infrastructure.Data;
using RM.Infrastructure.Database;
using Testcontainers.PostgreSql;

namespace RM.Testing;

public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _pgContainer;
    private DbInitializer _initializer;

    private string TestConnectionString => _pgContainer.GetConnectionString();
    private IConfiguration Configuration { get; set; }

    public ApiFactory()
    {
        _pgContainer = new PostgreSqlBuilder().WithDatabase("recommendationgames_test").WithUsername("recom").WithPassword("abc123").WithImage("postgres:17").Build();

        Configuration = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
    }

    public async Task InitializeAsync()
    {
        await _pgContainer.StartAsync();
        var factory = new DbConnectionFactory(TestConnectionString);
        _initializer = new DbInitializer(factory);
        await _initializer.InitializeDbAsync();

        using var conn = await factory.GetConnectionAsync(CancellationToken.None);
        await conn.ExecuteAsync(@"
            TRUNCATE TABLE ratings, genres, games, users
            RESTART IDENTITY CASCADE;
        ");
    }

    public new async Task DisposeAsync() => await _pgContainer.StopAsync();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddHttpContextAccessor();
            services.AddSingleton(Configuration);

            // Database
            services.RemoveAll<IDbConnectionFactory>();
            services.AddSingleton<IDbConnectionFactory>(_ => new DbConnectionFactory(TestConnectionString));

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IGamesRepository, GamesRepository>();
            services.AddScoped<IRatingRepository, RatingRepository>();
            services.AddValidatorsFromAssemblyContaining<IApplicationMarker>();

            // Game                                                          
            services.AddScoped<IGameUseCase, GameUseCase>();
            services.AddScoped<IRatingUseCase, RatingUseCase>();

            // Hasher                                                                                                          
            services.AddScoped<IPasswordHasher<UserRegisterRequestDto>, PasswordHasher<UserRegisterRequestDto>>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<IHasherService, HasherService>();

            // Authentication                 
            services.AddHttpContextAccessor();
            services.AddScoped<IUserValidationService, UserValidationService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<AuthTokenUseCase, AuthTokenUseCase>();
            services.AddScoped<IAuthenticateUserUseCase, AuthenticateUserUseCase>();
            
            services.AddAuthentication("TestScheme")
                    .AddScheme<AuthenticationSchemeOptions, AuthenticationHandlerTest>("TestScheme", options => { });
        });
    }
}