using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using App.Dtos.Authentication.Request;
using App.Dtos.Authentication.Response;
using App.Interfaces;
using App.Interfaces.Authentication;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RM.Domain.Entities;
using RM.Presentation.Routes;

namespace RM.Testing;

public class TokenServiceTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _apiFactory;

    public TokenServiceTests(ApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
    }

    [Fact]
    public async Task WhenNotBeingAuthorized_ThenGiveMeUnauthorized()
    {
        var client = await _apiFactory.CreateClient().GetAsync(AuthEndpoints.AUTHORIZED);
        
        client.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [InlineData("fede", "123456", "fede@gmail.com", "Admin", true)]
    [InlineData("hola", "123456", "hola@gmail.com", "Admin", true)]
    [Theory]
    public async Task WhenBeingAuthorized_ThenGiveMeOk(string username, string hashedPassword, string email, string role, bool trustedUser)
    {
        var scope = _apiFactory.Services.CreateScope();
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();

        tokenService.Should().NotBeNull();

        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username = username,
            HashedPassword = hashedPassword, // must be hashed, but it's not the point of this test
            Email = email,
            Role = role,
            TrustedUser = trustedUser
        };

        TokenResponseDto token = await tokenService.CreateTokenResponse(user);

        token.Should().NotBeNull();

        token.AccessToken.Should().NotBeNull();
        token.RefreshToken.Should().NotBeNull();

        HttpClient client = _apiFactory.CreateClient();

        client.Should().NotBeNull();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

        var response = await client.GetAsync(AuthEndpoints.AUTHORIZED);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task WhenRefreshToken_ThenGiveMeOk()
    {
        using var scope = _apiFactory.Services.CreateScope();
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username = "fede",
            HashedPassword = "123456",
            Email = "fede@gmail.com",
            Role = "Admin",
            TrustedUser = true
        };
        
        await userRepository.CreateUserAsync(user);

        TokenResponseDto firstDto = await tokenService.CreateTokenResponse(user);
        firstDto.Should().NotBeNull();
        firstDto.AccessToken.Should().NotBeNull();
        firstDto.RefreshToken.Should().NotBeNull();
        
        DateTime utcNow = DateTime.UtcNow;

        User? dbUser = await userRepository.GetUserById(user.UserId);
        dbUser.Should().NotBeNull();
        dbUser.RefreshToken.Should().Be(firstDto.RefreshToken);
        dbUser.RefreshTokenExpirationTimeUtc.Should().BeOnOrAfter(utcNow);

        var refreshRequest = new RefreshTokenRequestDto
        {
            UserId = user.UserId,
            RefreshToken = firstDto.RefreshToken
        };
        
        TokenResponseDto? secondDto = await tokenService.RefreshTokenAsync(refreshRequest);
        secondDto.Should().NotBeNull();
        secondDto.AccessToken.Should().NotBeNull().And.NotBe(firstDto.AccessToken);
        secondDto.RefreshToken.Should().NotBeNull().And.NotBe(firstDto.RefreshToken);

        User? updatedUser = await userRepository.GetUserById(user.UserId);
        updatedUser.Should().NotBeNull();
        updatedUser.RefreshToken.Should().Be(secondDto.RefreshToken);
        updatedUser.RefreshTokenExpirationTimeUtc.Should().BeOnOrAfter(utcNow);

        var client = _apiFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", secondDto.AccessToken);

        var response = await client.GetAsync(AuthEndpoints.AUTHORIZED);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}