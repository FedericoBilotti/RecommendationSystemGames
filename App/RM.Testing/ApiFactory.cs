using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using App.Auth;
using App.Interfaces;
using App.Interfaces.Authentication;
using App.Services.Authenticate;
using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using RM.Infrastructure.Data;
using RM.Infrastructure.Database;
using RM.Presentation;
using Testcontainers.PostgreSql;

namespace RM.Testing;

public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _pgContainer;
    private DbInitializer _initializer;

    public string TestConnectionString => _pgContainer.GetConnectionString();
    
    public IConfiguration Configuration { get; private set; }
    
    public ApiFactory()
    {
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
        
        using var conn = await factory.GetConnectionAsync(CancellationToken.None);
        await conn.ExecuteAsync(@"
            TRUNCATE TABLE ratings, genres, games, users
            RESTART IDENTITY CASCADE;
        ");
    }

    public new async Task DisposeAsync() => await _pgContainer.StopAsync();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IDbConnectionFactory>();
            services.AddSingleton<IDbConnectionFactory>(_ => new DbConnectionFactory(TestConnectionString));

            services.AddHttpContextAccessor();
            
            services.AddSingleton(Configuration);
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITokenService, TokenService>();
            
            // services.AddAuthorization(options =>
            // {
            //     options.AddPolicy(AuthConstants.ADMIN_ROLE, 
            //             policy => policy.RequireClaim(AuthConstants.ADMIN_CLAIM, "true"));
            //     options.AddPolicy(AuthConstants.TRUSTED_ROLE,
            //             policy => policy.RequireAssertion(x =>
            //                     x.User.HasClaim(m => m is { Type: AuthConstants.ADMIN_CLAIM, Value: "true" }) || 
            //                     x.User.HasClaim(m => m is { Type: AuthConstants.TRUSTED_CLAIM, Value: "true" })));
            // });
            //
            // services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            // {
            //     options.TokenValidationParameters = new TokenValidationParameters
            //     {
            //         ValidateIssuer = true,
            //         ValidIssuer = Configuration["AppSettings:Issuer"],
            //         ValidateAudience = true,
            //         ValidAudience = Configuration["AppSettings:Audience"],
            //         ValidateLifetime = true,
            //         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["AppSettings:Token"]!)),
            //         ValidateIssuerSigningKey = true
            //     };
            //
            //     options.Events = new JwtBearerEvents
            //     {
            //         OnMessageReceived = context =>
            //         {
            //             context.Token = context.Request.Cookies[TokenConstants.ACCESS_TOKEN];
            //             return Task.CompletedTask;
            //         }
            //     };
            // });
        });
    }
}