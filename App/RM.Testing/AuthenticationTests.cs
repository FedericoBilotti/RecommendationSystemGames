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
    public async Task WhenNotBeingAuthorized_ThenGiveMeUnauthorized()
    {
        var client = await _apiFactory.CreateClient().GetAsync(AuthEndpoints.AUTHORIZED);

        client.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData("fede", "123456", "fede@gmail.com", "Admin", true)]
    [InlineData("hola", "123456", "hola@gmail.com", "Admin", true)]
    public async Task WhenBeingAuthorized_ThenGiveMeOk(string username, string hashedPassword, string email, string role, bool trustedUser)
    {
        var token = await CreateToken(username, hashedPassword, email, role, trustedUser);

        HttpClient client = _apiFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

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

        HttpResponseMessage response = await _apiFactory.CreateClient().PostAsync(AuthEndpoints.Auth.REGISTER, JsonContent.Create(requestUserDto));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task WhenRefreshToken_ThenGiveMeOk()
    {
        var client = _apiFactory.CreateClient(new WebApplicationFactoryClientOptions {
            HandleCookies     = true,
            AllowAutoRedirect = false
        });
        
        var loginDto = new UserLoginRequestDto {
            Username = "pepe",
            Password = "pepe123456"
        };
        
        var loginResp = await client.PostAsJsonAsync(AuthEndpoints.Auth.LOGIN, loginDto);
        loginResp.EnsureSuccessStatusCode();
        
        var res = await loginResp.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine(res);

        if (loginResp.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            foreach (var cookieHeader in cookies)
            {
                _testOutputHelper.WriteLine("Cookies " + cookieHeader);
            }
        }
        
        var refreshResp = await client.GetAsync(AuthEndpoints.Auth.REFRESH_TOKEN);
        var refresh = await refreshResp.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine(refresh);
        
        refreshResp.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var refreshed = await refreshResp.Content.ReadFromJsonAsync<TokenResponseDto>();
        refreshed.Should().NotBeNull();
        refreshed.RefreshToken.Should().NotBeNullOrEmpty();
        _testOutputHelper.WriteLine(refreshed.RefreshToken);
        
        // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", refreshed.AccessToken);
        // var protectedResp = await client.GetAsync(AuthEndpoints.AUTHORIZED);
        // protectedResp.StatusCode.Should().Be(HttpStatusCode.OK);
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