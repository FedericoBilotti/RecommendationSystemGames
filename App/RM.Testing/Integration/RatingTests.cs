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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using RM.Domain.Entities;
using RM.Presentation.Routes;
using Xunit.Abstractions;

namespace RM.Testing.Integration;

public class RatingTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _apiFactory;
    private readonly ITestOutputHelper _testOutputHelper;

    public RatingTests(ApiFactory apiFactory, ITestOutputHelper testOutputHelper)
    {
        _apiFactory = apiFactory;
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(5, "Denos 1")]
    [InlineData(3, "Denos 2")]
    [InlineData(1, "Denos 3")]
    public async Task WhenTryingToRateAGame_ThenGiveMeOk(int rating, string title)
    {
        var client = await GetClientWithAuth();

        var gameResponseDto = await CreateGame(title, client);

        var ratingRequestDto = new GameRateRequestDto { Rating = rating };

        var gameUpdateResponseDto = await client.PutAsJsonAsync(ApiEndpoints.V1.Games.RATE.Replace("{id:Guid}", gameResponseDto.GameId.ToString()), ratingRequestDto);
        gameUpdateResponseDto.EnsureSuccessStatusCode();
        gameUpdateResponseDto.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Theory]
    [InlineData(6, "Denos 4")]
    [InlineData(0, "Denos 5")]
    [InlineData(-1, "Denos 6")]
    public async Task WhenTryingToRateAGame_ThenGiveMeBadRequest(int rating, string title)
    {
        var client = await GetClientWithAuth();

        var gameResponseDto = await CreateGame(title, client);

        var ratingRequestDto = new GameRateRequestDto { Rating = rating };

        var gameUpdateResponseDto = await client.PutAsJsonAsync(ApiEndpoints.V1.Games.RATE.Replace("{id:Guid}", gameResponseDto.GameId.ToString()), ratingRequestDto);
        gameUpdateResponseDto.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task WhenTryingToRateAGame_ThenGiveMeNotFound()
    {
        var client = await GetClientWithAuth();

        var gameResponseDto = await CreateGame("CreateGame", client);

        var ratingRequestDto = new GameRateRequestDto { Rating = 4 };

        var gameUpdateResponseDto = await client.PutAsJsonAsync(ApiEndpoints.V1.Games.RATE.Replace("{id:Guid}", "1"), ratingRequestDto);
        gameUpdateResponseDto.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task WhenTryingToDeleteRating_ThenGiveMeOk()
    {
        var client = await GetClientWithAuth();

        var gameResponseDto = await CreateGame("title 2", client);

        var ratingRequestDto = new GameRateRequestDto { Rating = 4 };

        var gameUpdateResponseDto = await client.PutAsJsonAsync(ApiEndpoints.V1.Games.RATE.Replace("{id:Guid}", gameResponseDto.GameId.ToString()), ratingRequestDto);
        gameUpdateResponseDto.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var deleteRating = await client.DeleteAsync(ApiEndpoints.V1.Games.DELETE_RATE.Replace("{id:Guid}", gameResponseDto.GameId.ToString()));
        deleteRating.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task WhenTryingToDeleteRating_ThenGiveMeNotFound()
    {
        var client = await GetClientWithAuth();

        var gameResponseDto = await CreateGame("title", client);

        var ratingRequestDto = new GameRateRequestDto { Rating = 4 };

        var gameUpdateResponseDto = await client.PutAsJsonAsync(ApiEndpoints.V1.Games.RATE.Replace("{id:Guid}", gameResponseDto.GameId.ToString()), ratingRequestDto);
        gameUpdateResponseDto.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var deleteRating = await client.DeleteAsync(ApiEndpoints.V1.Games.DELETE_RATE.Replace("{id:Guid}", "1"));
        deleteRating.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task WhenTryingToGetUserRatings_ThenGiveMeOk()
    {
        var client = await GetClientWithAuth();

        var gameResponseDto = await CreateGame("magic", client);

        var ratingRequestDto = new GameRateRequestDto { Rating = 4 };

        await client.PutAsJsonAsync(ApiEndpoints.V1.Games.RATE.Replace("{id:Guid}", gameResponseDto.GameId.ToString()), ratingRequestDto);
        
        var response = await client.GetAsync(ApiEndpoints.V1.Ratings.GET_USER_RATINGS);
        var t = await response.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine("Eh? " + t);
        
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
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

    private async Task<HttpClient> GetClientWithAuth(string username = "pepeTest", string email = "pepeTest@gmail.com", string role = AuthConstants.ADMIN_ROLE)
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