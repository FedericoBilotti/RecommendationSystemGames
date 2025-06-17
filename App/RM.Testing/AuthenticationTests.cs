using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using App.Auth;
using App.Dtos.Authentication.Request;
using App.Dtos.Authentication.Response;
using App.Interfaces;
using App.Interfaces.Authentication;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RM.Domain.Entities;
using RM.Presentation.Routes;
using Xunit.Abstractions;

namespace RM.Testing;

public class AuthenticationTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _apiFactory;
    private readonly ITestOutputHelper _testOutputHelper;

    public AuthenticationTests(ApiFactory apiFactory, ITestOutputHelper testOutputHelper)
    {
        _apiFactory = apiFactory;
        _testOutputHelper = testOutputHelper;
    }
    
    [Fact]
    public async Task WhenBeingAuthorized_ThenGiveMeOk()
    {
        HttpClient client = _apiFactory.CreateClient();

        var response = await client.GetAsync(AuthEndpoints.AUTHORIZED);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task WhenTryingToRegister_ThenGiveMeOk()
    {
        var requestUserDto = new UserRegisterRequestDto
        {
            Username = "pepe",
            Email = "pepe@gmail.com",
            Password = "pepe123456"
        };

        HttpResponseMessage response = await _apiFactory.CreateClient().PostAsJsonAsync(AuthEndpoints.Auth.REGISTER, requestUserDto);

        response.EnsureSuccessStatusCode();

        var matchResponse = await response.Content.ReadFromJsonAsync<UserResponseDto>();
        matchResponse.Should().NotBeNull();
        matchResponse.UserId.Should().NotBeEmpty();
        matchResponse.Username.Should().Be(requestUserDto.Username);
        matchResponse.Email.Should().Be(requestUserDto.Email);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task WhenRefreshToken_ThenGiveMeOk()
    {
        var client = _apiFactory.CreateClient(new WebApplicationFactoryClientOptions
        {
            HandleCookies     = true,
            AllowAutoRedirect = false
        });

        var loginDto = new UserLoginRequestDto {
            Username = "pepe",
            Password = "pepe123456"
        };
        
        var loginResp = await client.PostAsJsonAsync(AuthEndpoints.Auth.LOGIN, loginDto);
        var response = await loginResp.Content.ReadFromJsonAsync<TokenResponseDto>();
        response.Should().NotBeNull();
        response.AccessToken.Should().NotBeNullOrEmpty();
        response.RefreshToken.Should().NotBeNullOrEmpty();
        Console.WriteLine(response.AccessToken);
        Console.WriteLine(response.RefreshToken);
        loginResp.EnsureSuccessStatusCode();

        var matchResponse = await loginResp.Content.ReadFromJsonAsync<UserResponseDto>();
        matchResponse.Should().NotBeNull();
        matchResponse.UserId.Should().NotBeEmpty();
        matchResponse.Username.Should().Be(loginDto.Username);
        matchResponse.Email.Should().Be(loginDto.Email);
        

        var refreshResp = await client.PostAsJsonAsync(AuthEndpoints.Auth.REFRESH_TOKEN_ID, matchResponse.UserId);
        // refreshResp.EnsureSuccessStatusCode();
        var refreshed = await refreshResp.Content.ReadFromJsonAsync<TokenResponseDto>();
        refreshed.Should().NotBeNull();
        refreshed.RefreshToken.Should().NotBeNullOrEmpty();
        _testOutputHelper.WriteLine(refreshed.RefreshToken);
        refreshResp.StatusCode.Should().Be(HttpStatusCode.OK);

        var tokenDto = await refreshResp.Content.ReadFromJsonAsync<TokenResponseDto>();
        tokenDto!.RefreshToken.Should().NotBeNullOrEmpty();
    }

    private async Task<TokenResponseDto> CreateToken(string username, string hashedPassword, string email, string role, bool trustedUser)
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

        return token;
    }
}