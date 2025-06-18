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
using RM.Presentation.Controllers.Authentication;
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
    public async Task WhenNotBeingAuthorized_ThenGiveMeUnauthorized()
    {
        HttpClient client = _apiFactory.CreateClient();
        HttpResponseMessage response = await client.GetAsync(AuthEndpoints.AUTHORIZED);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task WhenBeingAuthorized_ThenGiveMeOk()
    {
        TokenResponseDto token = await GetJwtAsync(Guid.NewGuid(), username: "pepeTest", role: AuthConstants.USER_ROLE, email: "pepeTest@gmail.com");

        HttpClient client = _apiFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

        HttpResponseMessage response = await client.GetAsync(AuthEndpoints.AUTHORIZED);
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
        var res = await response.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine("Register: " + res);
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

        // Register
        
        var requestRegisterUserDto = new UserRegisterRequestDto
        {
            Username = "pepePepon",
            Email = "pepePepon@gmail.com",
            Password = "pepe123456"
        };

        HttpResponseMessage responseRegister = await client.PostAsJsonAsync(AuthEndpoints.Auth.REGISTER, requestRegisterUserDto);
        responseRegister.EnsureSuccessStatusCode();

        var matchRegister = await responseRegister.Content.ReadFromJsonAsync<UserResponseDto>();
        matchRegister.Should().NotBeNull();
        matchRegister.UserId.Should().NotBeEmpty();
        matchRegister.Username.Should().Be(requestRegisterUserDto.Username);
        matchRegister.Email.Should().Be(requestRegisterUserDto.Email);
        responseRegister.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Login

        var requestLoginUserDto = new UserLoginRequestDto
        {
            Email = requestRegisterUserDto.Email,
            Username = requestRegisterUserDto.Username,
            Password = requestRegisterUserDto.Password
        };
        
        HttpResponseMessage responseLogin = await client.PostAsJsonAsync(AuthEndpoints.Auth.LOGIN, requestLoginUserDto);
        responseLogin.EnsureSuccessStatusCode();
        
        var matchLogin = await responseLogin.Content.ReadFromJsonAsync<TokenResponseDto>();
        matchLogin.Should().NotBeNull();
        matchLogin.AccessToken.Should().NotBeEmpty();
        matchLogin.RefreshToken.Should().NotBeEmpty();
        responseLogin.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Refresh token
        
        var requestRefreshTokenDto = new RefreshTokenRequestDto { UserId = matchRegister.UserId, RefreshToken = matchLogin.RefreshToken };
        
        HttpResponseMessage responseRefresh = await client.PostAsJsonAsync(AuthEndpoints.Auth.REFRESH_TOKEN, requestRefreshTokenDto);
        responseRefresh.EnsureSuccessStatusCode();
        
        var matchRefresh = await responseRefresh.Content.ReadFromJsonAsync<TokenResponseDto>();
        matchRefresh.Should().NotBeNull();
        matchRefresh.AccessToken.Should().NotBeEmpty();
        matchRefresh.RefreshToken.Should().NotBeEmpty();
        responseRefresh.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task WhenBeingAdmin_ThenGiveMeOk()
    {
        TokenResponseDto token = await GetJwtAsync(Guid.NewGuid(), username: "pepeTest", role: AuthConstants.ADMIN_ROLE, email: "pepeTest@gmail.com");

        HttpClient client = _apiFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

        HttpResponseMessage response = await client.GetAsync(AuthEndpoints.ADMIN);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task WhenNotBeingAdmin_ThenGiveMeUnauthorized()
    {
        TokenResponseDto token = await GetJwtAsync(Guid.NewGuid(), username: "pepeTest", role: AuthConstants.USER_ROLE, email: "pepeTest@gmail.com");

        HttpClient client = _apiFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

        HttpResponseMessage response = await client.GetAsync(AuthEndpoints.ADMIN);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task WhenBeingTrustedUser_ThenGiveMeOk()
    {
        TokenResponseDto token = await GetJwtAsync(Guid.NewGuid(), username: "pepeTest", role: AuthConstants.TRUSTED_ROLE, email: "pepeTest@gmail.com");

        HttpClient client = _apiFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

        HttpResponseMessage response = await client.GetAsync(AuthEndpoints.TRUSTED_USER);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task WhenNotBeingTrustedUser_ThenGiveMeUnauthorized()
    {
        TokenResponseDto token = await GetJwtAsync(Guid.NewGuid(), username: "pepeTest", role: AuthConstants.USER_ROLE, email: "pepeTest@gmail.com");

        HttpClient client = _apiFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

        HttpResponseMessage response = await client.GetAsync(AuthEndpoints.TRUSTED_USER);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
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