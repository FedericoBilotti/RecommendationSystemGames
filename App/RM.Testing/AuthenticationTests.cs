using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using App.Auth;
using App.Dtos.Authentication.Request;
using App.Dtos.Authentication.Response;
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
        var client = _apiFactory.CreateClient();
        var userRegisterRequestDto = new UserRegisterRequestDto
        {
            Username = "pepe",
            Email = "pepe@gmail.com",
            Password = "pepe123456"
        };
        
        await RegisterUser(client, userRegisterRequestDto);
    }

    [Fact]
    public async Task WhenTryingToLogin_ThenGiveMeOk()
    {
        var client = _apiFactory.CreateClient();
        
        // Register
        
        var requestUserDto = new UserRegisterRequestDto
        {
            Username = "pepepepepepepe",
            Email = "pepepepepepe@gmail.com",
            Password = "pepe123456"
        };

        await RegisterUser(client, requestUserDto);

        // Login

        var requestLoginUserDto = new UserLoginRequestDto
        {
            Email = requestUserDto.Email,
            Username = requestUserDto.Username,
            Password = requestUserDto.Password
        };

        await LoginUser(client, requestLoginUserDto);
    }

    [Fact]
    public async Task WhenTryingToRefreshToken_ThenGiveMeOk()
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

        var responseRegister = await RegisterUser(client, requestRegisterUserDto);
        
        // Login

        var requestLoginUserDto = new UserLoginRequestDto
        {
            Email = requestRegisterUserDto.Email,
            Username = requestRegisterUserDto.Username,
            Password = requestRegisterUserDto.Password
        };
        
        var responseLogin = await LoginUser(client, requestLoginUserDto);
        
        // Refresh token
        
        var requestRefreshTokenDto = new RefreshTokenRequestDto { UserId = responseRegister.UserId, RefreshToken = responseLogin.RefreshToken };
        
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
    public async Task WhenNotBeingAdmin_ThenGiveMeForbidden()
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
    public async Task WhenNotBeingTrustedUser_ThenGiveMeForbidden()
    {
        TokenResponseDto token = await GetJwtAsync(Guid.NewGuid(), username: "pepeTest", role: AuthConstants.USER_ROLE, email: "pepeTest@gmail.com");

        HttpClient client = _apiFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

        HttpResponseMessage response = await client.GetAsync(AuthEndpoints.TRUSTED_USER);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<UserResponseDto> RegisterUser(HttpClient client, UserRegisterRequestDto registerRequestUserDto)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(AuthEndpoints.Auth.REGISTER, registerRequestUserDto);
        response.EnsureSuccessStatusCode();

        var matchResponse = await response.Content.ReadFromJsonAsync<UserResponseDto>();
        matchResponse.Should().NotBeNull();
        matchResponse.UserId.Should().NotBeEmpty();
        matchResponse.Username.Should().Be(registerRequestUserDto.Username);
        matchResponse.Email.Should().Be(registerRequestUserDto.Email);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        return matchResponse;
    }

    private async Task<TokenResponseDto> LoginUser(HttpClient client, UserLoginRequestDto loginRequestUserDto)
    {
        var responseLogin = await client.PostAsJsonAsync(AuthEndpoints.Auth.LOGIN, loginRequestUserDto);
        responseLogin.EnsureSuccessStatusCode();
        
        var matchLogin = await responseLogin.Content.ReadFromJsonAsync<TokenResponseDto>();
        matchLogin.Should().NotBeNull();
        matchLogin.AccessToken.Should().NotBeEmpty();
        matchLogin.RefreshToken.Should().NotBeEmpty();
        responseLogin.StatusCode.Should().Be(HttpStatusCode.OK);
        return matchLogin;
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