using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using App.Auth;
using App.Dtos.Authentication.Response;
using App.Dtos.Games.Requests;
using App.Dtos.Games.Responses;
using App.Interfaces.Authentication;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RM.Domain.Entities;
using RM.Presentation.Routes;
using Xunit.Abstractions;

namespace RM.Testing.Integration;

public class GameTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _apiFactory;
    private readonly ITestOutputHelper _testOutputHelper;

    public GameTests(ApiFactory apiFactory, ITestOutputHelper testOutputHelper)
    {
        _apiFactory = apiFactory;
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData("pepeTest", "pepeTest@gmail.com", AuthConstants.TRUSTED_ROLE, "Dummy")]
    [InlineData("pepeTesting", "pepeTesting@gmail.com", AuthConstants.ADMIN_ROLE, "Other Dummy Game")]
    public async Task WhenTryingToCreateAGame_ThenGiveMeCreated(string username, string email, string role, string title)
    {
        HttpClient client = await GetClientAuthorized(username, email, role);

        await CreateGame(title, client);
    }

    private static async Task<GameResponseDto> CreateGame(string title, HttpClient client)
    {
        var gameRequestDto = new CreateGameRequestDto
        {
            Title = title,
            Description = "Dummy description",
            YearOfRelease = 2022,
            Genre = new List<string> { "Action", "Adventure" }
        };

        var response = await client.PostAsJsonAsync(ApiEndpoints.V1.Games.CREATE, gameRequestDto);
        response.EnsureSuccessStatusCode();
        
        var matchResponse = await response.Content.ReadFromJsonAsync<GameResponseDto>();
        matchResponse.Should().NotBeNull();
        matchResponse.GameId.Should().NotBeEmpty();
        matchResponse.Title.Should().Be(gameRequestDto.Title);
        matchResponse.Description.Should().Be(gameRequestDto.Description);
        matchResponse.YearOfRelease.Should().Be(gameRequestDto.YearOfRelease);
        matchResponse.Genre.Should().BeEquivalentTo(gameRequestDto.Genre);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        return matchResponse;
    }
    
    [Fact]
    public async Task WhenTryingToCreateAGame_AndNotBeingTrustedRole_ThenGiveMeForbidden()
    {
        var client = await GetClientAuthorized(role: AuthConstants.USER_ROLE);
        
        var gameRequestDto = new CreateGameRequestDto
        {
            Title = "Dummy",
            Description = "Dummy description",
            YearOfRelease = 2022,
            Genre = new List<string> { "Action", "Adventure" }
        };

        var response = await client.PostAsJsonAsync(ApiEndpoints.V1.Games.CREATE, gameRequestDto);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task WhenTryingToGetAGame_ThenGiveMeOk()
    {
        var client = await GetClientAuthorized();

        var gameResponseDto = await CreateGame("The Dummy", client);
        
        var response = await client.GetAsync(ApiEndpoints.V1.Games.GET.Replace("{idOrSlug}", gameResponseDto.GameId.ToString()));
        response.EnsureSuccessStatusCode();
        
        var matchResponse = await response.Content.ReadFromJsonAsync<GameResponseDto>();
        matchResponse.Should().NotBeNull();
        matchResponse.GameId.Should().Be(gameResponseDto.GameId);
        matchResponse.Title.Should().Be(gameResponseDto.Title);
        matchResponse.Description.Should().Be(gameResponseDto.Description);
        matchResponse.YearOfRelease.Should().Be(gameResponseDto.YearOfRelease);
        matchResponse.Genre.Should().BeEquivalentTo(gameResponseDto.Genre);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task WhenTryingToGetAGame_AndNotBeingAuthorized_ThenGiveMeUnauthorized()
    {
        var client = _apiFactory.CreateClient();
        
        var response = await client.GetAsync(ApiEndpoints.V1.Games.GET.Replace("{idOrSlug}", "1")); // Doesn't matter the id, cause it's unauthorized
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task WhenTryingToGetAllGames_AndNotBeingAuthorized_ThenGiveMeUnauthorized()
    {
        var client = _apiFactory.CreateClient();
        
        var response = await client.GetAsync(ApiEndpoints.V1.Games.GET_ALL);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData(3, 5)]
    [InlineData(1, 25)]
    public async Task WhenTryingToGetAllGames_ThenGiveMeOk(int page, int pageSize)
    {
        var client = await GetClientAuthorized(role: AuthConstants.USER_ROLE);

        var url = $"{ApiEndpoints.V1.Games.GET_ALL}?page={page}&pageSize={pageSize}";
        var response = await client.GetAsync(url);
        
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Theory]
    [InlineData(3, 26)]
    [InlineData(1, -1)]
    [InlineData(0, 20)]
    [InlineData(-1, 5)]
    public async Task WhenTryingToGetAllGames_ThenGiveMeBadRequest(int page, int pageSize)
    {
        var client = await GetClientAuthorized(role: AuthConstants.USER_ROLE);

        var url = $"{ApiEndpoints.V1.Games.GET_ALL}?page={page}&pageSize={pageSize}";
        
        var response = await client.GetAsync(url);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(6, 5, "", 2022, "title")]
    [InlineData(1, 5, "Dummy", 2022,"title")]
    [InlineData(5, 25, "y", 2022, "yearofrelease")]
    [InlineData(4, 5, "mmy", 2022, "-title")]
    [InlineData(1, 15, "Dum", 2022, "-yearofrelease")]
    public async Task WhenTryingToGetAllGames_FilteringAndSorting_ThenGiveMeOk(int page, int pageSize, string title, int yearOfRelease, string sortBy)
    {
        var client = await GetClientAuthorized(role: AuthConstants.USER_ROLE);

        var url = $"{ApiEndpoints.V1.Games.GET_ALL}?sortBy={sortBy}&page={page}&pageSize={pageSize}&title={title}&yearOfRelease={yearOfRelease}";
        var response = await client.GetAsync(url);
        
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Theory]
    [InlineData(6, 5, "", 2022, "tile")]
    [InlineData(4, 5, "mmy", 2022, "-tite")]
    [InlineData(5, 25, "y", 2022, "yearofreleas")]
    [InlineData(5, 25, "y", 2022, "-yearofreleas")]
    public async Task WhenTryingToGetAllGames_FilteringAndSorting_ThenGiveMeBadRequest(int page, int pageSize, string title, int yearOfRelease, string sortBy)
    {
        var client = await GetClientAuthorized(role: AuthConstants.USER_ROLE);

        var url = $"{ApiEndpoints.V1.Games.GET_ALL}?sortBy={sortBy}&page={page}&pageSize={pageSize}&title={title}&yearOfRelease={yearOfRelease}";
        var response = await client.GetAsync(url);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WhenTryingToUpdate_ThenGiveMeOk()
    {
        var client = await GetClientAuthorized();
        
        var gameResponseDto = await CreateGame("Denos", client);
        
        var updateGameRequestDto = new UpdateGameRequestDto
        {
            Title = "Denos 2",
            Description = "Dummy description 2",
            YearOfRelease = 2023,
            Genre = new List<string> { "Action", "Adventure" }
        };

        var response = await client.PutAsJsonAsync(ApiEndpoints.V1.Games.UPDATE.Replace("{id:Guid}", gameResponseDto.GameId.ToString()), updateGameRequestDto);
        response.EnsureSuccessStatusCode();
        
        var matchResponse = await response.Content.ReadFromJsonAsync<GameResponseDto>();
        matchResponse.Should().NotBeNull();
        matchResponse.GameId.Should().Be(gameResponseDto.GameId);
        matchResponse.Title.Should().Be(updateGameRequestDto.Title);
        matchResponse.Description.Should().Be(updateGameRequestDto.Description);
        matchResponse.YearOfRelease.Should().Be(updateGameRequestDto.YearOfRelease);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task WhenTryingToUpdate_AndBeingUser_ThenGiveMeForbidden()
    {
        var client = await GetClientAuthorized(role: AuthConstants.USER_ROLE);
        
        var gameCreated = await CreateGame("Another Dummy Game 2", await GetClientAuthorized());
        
        var updateGameRequestDto = new UpdateGameRequestDto
        {
            Title = "Another Dummy Game 3",
            Description = "Dummy description 2",
            YearOfRelease = 2023,
            Genre = new List<string> { "Action", "Adventure" }
        };

        var response = await client.PutAsJsonAsync(ApiEndpoints.V1.Games.UPDATE.Replace("{id:Guid}", gameCreated.GameId.ToString()), updateGameRequestDto);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task WhenTryingToUpdate_AndNotBeingAuthorized_ThenGiveMeUnauthorized()
    {
        var client = _apiFactory.CreateClient();
        
        var gameCreated = await CreateGame("Another Dummy Game", await GetClientAuthorized());
        
        var updateGameRequestDto = new UpdateGameRequestDto
        {
            Title = "Another Dummy Game 2",
            Description = "Dummy description 2",
            YearOfRelease = 2023,
            Genre = new List<string> { "Action", "Adventure" }
        };

        var response = await client.PutAsJsonAsync(ApiEndpoints.V1.Games.UPDATE.Replace("{id:Guid}", gameCreated.GameId.ToString()), updateGameRequestDto);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task WhenTryingToUpdate_ThenGiveMeNotFound()
    {
        var client = _apiFactory.CreateClient();
        
        var updateGameRequestDto = new UpdateGameRequestDto
        {
            Title = "Another Dummy Game 2",
            Description = "Dummy description 2",
            YearOfRelease = 2023,
            Genre = new List<string> { "Action", "Adventure" }
        };

        var response = await client.PutAsJsonAsync(ApiEndpoints.V1.Games.UPDATE.Replace("{id:Guid}", "2"), updateGameRequestDto);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<HttpClient> GetClientAuthorized(string username = "pepeTest", string email = "pepeTest@gmail.com", string role = AuthConstants.ADMIN_ROLE)
    {
        TokenResponseDto token = await GetJwtAsync(Guid.NewGuid(), username: username, email: email, role: role);

        HttpClient client = _apiFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        return client;
    }
    
    private async Task<TokenResponseDto> GetJwtAsync(Guid userId, string username, string role, string email)
    {
        var user = new User
        {
            UserId = userId,
            Username = username,
            Email = email,
            Role = role,
            HashedPassword = "dummy"
        };

        using var scope = _apiFactory.Services.CreateScope();
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();

        TokenResponseDto tokenResponseDto = await tokenService.CreateTokenResponse(user);
        return tokenResponseDto;
    }
}