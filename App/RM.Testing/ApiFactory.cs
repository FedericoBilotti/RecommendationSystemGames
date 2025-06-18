using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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
using DotNetEnv;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
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
        var basePath = Directory.GetCurrentDirectory();
        var projectPath = Path.GetFullPath(Path.Combine(basePath, "..", "..", "..", ".."));
        var envPath = Path.Combine(projectPath, ".env");
        Env.Load(envPath);

        _pgContainer = new PostgreSqlBuilder()
                .WithDatabase("recommendationgames_test")
                .WithUsername("recom")
                .WithPassword("abc123")
                .WithImage("postgres:17")
                .Build();

        Configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
    }

    public async Task InitializeAsync()
    {
        await _pgContainer.StartAsync();
        var factory = new DbConnectionFactory(TestConnectionString);
        _initializer = new DbInitializer(factory);
        await _initializer.InitializeDbAsync();
    }

    public new async Task DisposeAsync() => await _pgContainer.StopAsync();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) => config.AddEnvironmentVariables());

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

            // services.AddAuthentication("TestScheme").AddScheme<AuthenticationSchemeOptions, AuthenticationHandlerTest>("TestScheme", _ => { });
        });
    }
}