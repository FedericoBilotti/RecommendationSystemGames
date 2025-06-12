using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using App.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RM.Infrastructure.Database;
using RM.Presentation;
using RM.Presentation.Routes;

namespace RM.Testing;

public class CustomWebAppFactory : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        IConfigurationRoot tempConfig = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();
        
        string testConnection = tempConfig
                .GetSection("ConnectionStrings")
                .GetValue<string>("TestDatabase")
                ?? throw new Exception("Test database connection string not found");
                       

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:DefaultConnection"] = testConnection
            }!);
        });

        builder.ConfigureServices(services =>
        {
            var dbFactoryDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDbConnectionFactory));
            if (dbFactoryDescriptor != null) services.Remove(dbFactoryDescriptor);

            services.AddSingleton<IDbConnectionFactory>(_ => new DbConnectionFactory(testConnection));
        });

        return base.CreateHost(builder);
    }
    
    [Fact]
    public async Task GetAllGamesAuthorized()
    {
        // Logic to get authorized
        
        HttpResponseMessage client = await CreateClient().GetAsync(ApiEndpoints.V1.Games.GET_ALL);

        Assert.Equal(HttpStatusCode.OK, client.StatusCode);
    }

    [Fact]
    public async Task GetAllGamesUnauthorized()
    {
        HttpResponseMessage client = await CreateClient().GetAsync(ApiEndpoints.V1.Games.GET_ALL);

        Assert.Equal(HttpStatusCode.Unauthorized, client.StatusCode);
    }
}